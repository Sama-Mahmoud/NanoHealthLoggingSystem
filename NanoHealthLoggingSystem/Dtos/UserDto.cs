namespace NanoHealthLoggingSystem.Dtos
{
    public class UserDto
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }
    }
    public class RegisterDto
    {
        public string Password { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string RoleId { get; set; }
    }
    public class LogInDto
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}
