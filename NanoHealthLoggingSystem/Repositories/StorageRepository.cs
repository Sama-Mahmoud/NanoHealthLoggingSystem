using NanoHealthLoggingSystem.Context;
using NanoHealthLoggingSystem.Dtos;
using NanoHealthLoggingSystem.Entities;
using NanoHealthLoggingSystem.IRepositories;

namespace NanoHealthLoggingSystem.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly LoggingContext _context;

        public StorageRepository(LoggingContext context)
        {
            _context = context;
        }
        public async Task<string> Add(StorageDto dto)
        {
            try
            {
                var Storage = new Storage { StorageType = dto.StorageType, StorageLink = dto.StorageLink };
                await _context.Storage.AddAsync(Storage);
                await _context.SaveChangesAsync();
                return dto.StorageType;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(string Id)
        {
            try
            {
                var Storage = _context.Storage.FirstOrDefault(s => s.StorageType == Id);
                if (Storage == null)
                {
                    throw new NullReferenceException("Doesnot Exist Storages");
                }
                _context.Storage.Remove(Storage);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StorageDto> Get(string id)
        {
            try
            {
                var Storage = _context.Storage.FirstOrDefault(s => s.StorageType == id);
                if (Storage == null)
                {
                    throw new NullReferenceException("Doesnot Exist Storages");
                }
                return new StorageDto {StorageType = Storage.StorageType , StorageLink = Storage.StorageLink}; 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ICollection<StorageDto>> GetAll()
        {
            try
            {
                var Storages = _context.Storage.ToList();
                if (Storages == null)
                {
                    throw new NullReferenceException("Doesnot Exist Storages");
                }
                var storageDtos = new List<StorageDto>();
                foreach (var item in Storages)
                {
                    storageDtos.Add(new StorageDto { StorageType = item.StorageType ,StorageLink = item.StorageLink});
                }
                return storageDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> Update(StorageDto dto)
        {
            try
            {
                var Storage = _context.Storage.FirstOrDefault(s => s.StorageType == dto.StorageType);
                if (Storage == null)
                {
                    throw new NullReferenceException("Doesnot Exist Storage");
                }
                Storage.StorageLink = dto.StorageLink;
                _context.Storage.Update(Storage);
                await _context.SaveChangesAsync();
                return Storage.StorageType;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
