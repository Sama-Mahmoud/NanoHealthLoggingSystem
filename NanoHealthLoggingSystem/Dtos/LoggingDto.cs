using NanoHealthLoggingSystem.Enums;

namespace NanoHealthLoggingSystem.Dtos
{
    public class LoggingDto
    {
        public string Service { get; set; } = null!;
        public string Message { get; set; } = null!;
        public Level Level { get; set; }
        public string StorageId { get; set; } = null!;
    }
    public class GetLoggingDto
    {
        public Guid Id { get; set; }
        public string Service { get; set; } = null!;
        public string Message { get; set; } = null!;
        public Level Level { get; set; }
        public DateTime Timestamp { get; set; }
        public string StorageId { get; set; } = null!;
    }
}
