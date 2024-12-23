using NanoHealthLoggingSystem.Entities;
using NanoHealthLoggingSystem.Enums;

namespace NanoHealthLoggingSystem.Enteties
{
    public class LoggingMetaData
    {
        public Guid Id { get; set; }
        public string Service { get; set; } = null!;
        public string Message { get; set; } = null!;
        public Level Level { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string StorageId { get; set; } = null!;
        public Storage Storage { get; set; } = null!;
    }
}
