using NanoHealthLoggingSystem.Dtos;

namespace NanoHealthLoggingSystem.IRepositories
{
    public interface ILoggingRepository
    {
        Task<Guid> Add(LoggingDto Dto);
        Task SaveInfileStreem(IFormFile file, Guid LoggingId);
        Task SaveInS3(IFormFile file, Guid LoggingId);
        ICollection<GetLoggingDto> GetAll(Dictionary<string, string> filters);
        Task<string> GetJsonfile(Guid Id);
    }
}
