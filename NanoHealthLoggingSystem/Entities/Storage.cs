using NanoHealthLoggingSystem.Enteties;
using System.ComponentModel.DataAnnotations;

namespace NanoHealthLoggingSystem.Entities
{
    public class Storage
    {
        [Key]
        public string StorageType { get; set; } = null!; 
        public string StorageLink { get; set; } = null!;
        public ICollection<LoggingMetaData>? MetaDatas { get; set; }
    }
}
