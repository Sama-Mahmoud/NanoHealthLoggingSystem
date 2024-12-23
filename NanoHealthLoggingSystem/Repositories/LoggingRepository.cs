using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NanoHealthLoggingSystem.Context;
using NanoHealthLoggingSystem.Dtos;
using NanoHealthLoggingSystem.Enteties;
using NanoHealthLoggingSystem.Enums;
using NanoHealthLoggingSystem.IRepositories;
using System.Net.Http.Headers;

namespace NanoHealthLoggingSystem.Repositories
{
    public class LoggingRepository : ILoggingRepository
    {
        private readonly LoggingContext _context;

        public LoggingRepository(LoggingContext context)
        {
            _context = context;
        }

        public async Task<Guid> Add(LoggingDto Dto)
        {
            var Meta = new LoggingMetaData
            {
                Level = Dto.Level,
                Message = Dto.Message,
                Service = Dto.Service,
                Timestamp = DateTime.Now,
                StorageId = Dto.StorageId
            };

            await _context.LoggingMetaData.AddAsync(Meta);
            await _context.SaveChangesAsync();
            return Meta.Id;
        }
        public async Task SaveInfileStreem(IFormFile file, Guid LoggingId)
        {
            var meta = _context.LoggingMetaData.FirstOrDefault(l => l.Id == LoggingId);
            if (meta == null)
            {
                throw new ArgumentException("Invalid Logging ID.");
            }
            try
                {
                    if (file == null || file.Length == 0)
                    {
                        throw new ArgumentNullException("No file uploaded.");
                    }

                    var fileExtension = Path.GetExtension(file.FileName);
                    if (fileExtension == null || fileExtension.ToLower() != ".json")
                    {
                        throw new ArgumentException("Invalid file type. Only JSON files are allowed.");
                    }

                    var fileName = $"{LoggingId}{fileExtension}";

                    var filePath = Path.Combine("wwwroot", fileName);

                    var directoryPath = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                }
                catch (Exception ex)
                {

                    _context.LoggingMetaData.Remove(meta);
                    await _context.SaveChangesAsync();
                }
        
        }

        public async Task SaveInS3(IFormFile file, Guid LoggingId)
        {
            var storage = _context.Storage.FirstOrDefault(s=>s.StorageType.ToLower().Equals(StorageType.S3.ToString().ToLower()));
            if (storage == null)
                throw new NullReferenceException("No Provided Bucket");
            var Link = storage.StorageLink;

            var meta = _context.LoggingMetaData.FirstOrDefault(l => l.Id == LoggingId);
            if (meta == null)
            {
                throw new ArgumentException("Invalid Logging ID.");
            }

            try
            {
                // Validate the file
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentNullException("No file uploaded.");
                }

                var fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension == null || fileExtension.ToLower() != ".json")
                {
                    throw new ArgumentException("Invalid file type. Only JSON files are allowed.");
                }

                // Generate a unique key for the file in S3
                var fileKey = $"{LoggingId}{fileExtension}"; // e.g., "123e4567-e89b-12d3-a456-426614174000.json"

                // Prepare the S3 upload URL
                var uploadUrl = $"{Link}/{fileKey}";

                using (var client = new HttpClient())
                using (var content = new StreamContent(file.OpenReadStream()))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await client.PutAsync(uploadUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to upload file to S3: {response.ReasonPhrase}");
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle the error and clean up
                _context.LoggingMetaData.Remove(meta);
                await _context.SaveChangesAsync();

                throw new Exception($"An error occurred while saving the file to S3: {ex.Message}");
            }
        }
        public ICollection<GetLoggingDto> GetAll(Dictionary<string, string> filters)
        {
            var query = _context.LoggingMetaData.AsQueryable();

            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "service":
                        var service = filter.Value;
                        if (!string.IsNullOrEmpty(service))
                        {
                            query = query.Where(log => log.Service.ToLower().Contains(service.ToLower()));
                        }
                        break;

                    case "level":
                        if (Enum.TryParse<Level>(filter.Value, out var level))
                        {
                            query = query.Where(log => log.Level == level);
                        }
                        break;

                    case "start_time":
                        if (DateTime.TryParse(filter.Value, out var startTime))
                        {
                            query = query.Where(log => log.Timestamp >= startTime);
                        }
                        break;

                    case "end_time":
                        if (DateTime.TryParse(filter.Value, out var endTime))
                        {
                            query = query.Where(log => log.Timestamp <= endTime);
                        }
                        break;

                    default:
                        throw new ArgumentException($"Unsupported filter key: {filter.Key}");
                }
            }
            var Logs = new List<GetLoggingDto>();
            foreach (var item in query)
            {
                Logs.Add(new GetLoggingDto { Id = item.Id , Level = item.Level , Message = item.Message ,
                    Service = item.Service , StorageId = item.StorageId , Timestamp = item.Timestamp});
            }

            return Logs;

        }

        public async Task<string> GetJsonfile(Guid Id)
        {
            try
            {
                var Meta = _context.LoggingMetaData.FirstOrDefault(l => l.Id == Id);
                if (Meta == null)
                    throw new FileNotFoundException("Metadata not found for the given ID.");

                if (Meta.StorageId.ToLower().Equals(StorageType.Filesystem.ToString().ToLower()))
                {
                    var filePath = Path.Combine("wwwroot", Id + ".json");
                    if (!System.IO.File.Exists(filePath))
                    {
                        throw new FileNotFoundException("File not found in the file system.");
                    }

                    // Read the file as a stream
                    var fileContent = await System.IO.File.ReadAllTextAsync(filePath);

                    return fileContent;
                }
                else if (Meta.StorageId.ToLower().Equals(StorageType.S3.ToString().ToLower()))
                {
                    // Fetch the S3 bucket link from the storage configuration
                    var storage = _context.Storage.FirstOrDefault(s => s.StorageType.ToLower() == StorageType.S3.ToString().ToLower());
                    if (storage == null)
                    {
                        throw new InvalidOperationException("S3 storage configuration not found.");
                    }

                    var fileUrl = $"{storage.StorageLink}/{Meta.Id}.json";

                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(fileUrl);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new FileNotFoundException("File not found in S3 storage.");
                        }

                        // Get the file content as a stream
                        var fileStream = await response.Content.ReadAsStreamAsync();

                        // Create an IFormFile to return
                        var fileName = Path.GetFileName(new Uri(fileUrl).AbsolutePath);
                        var fileContent = await response.Content.ReadAsStringAsync();

                        return fileContent; 
                    }
                }
                else
                {
                    throw new NotSupportedException($"Unsupported storage type: {Meta.StorageId}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the file: {ex.Message}");
            }
        }
    }
}
