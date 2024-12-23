using NanoHealthLoggingSystem.Dtos;

namespace NanoHealthLoggingSystem.IRepositories
{
    public interface IStorageRepository
    {
        Task<string> Add(StorageDto dto);
        Task<string> Update(StorageDto dto);
        Task Delete(string Id);
        Task<StorageDto> Get(string id);
        Task<ICollection<StorageDto>> GetAll();
    }
}
