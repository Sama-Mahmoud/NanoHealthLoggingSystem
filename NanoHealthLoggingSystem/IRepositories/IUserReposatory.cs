using NanoHealthLoggingSystem.Dtos;

namespace NanoHealthLoggingSystem.IRepositories
{
    public interface IUserReposatory
    {
        public Task<UserDto> Register(RegisterDto user);
        public ICollection<RoleDto> GetRoles();
        public Task<UserDto> LogIn(LogInDto user);
    }
}
