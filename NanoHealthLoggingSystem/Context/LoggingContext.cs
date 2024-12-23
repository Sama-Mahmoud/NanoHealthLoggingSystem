using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NanoHealthLoggingSystem.Enteties;
using NanoHealthLoggingSystem.Entities;

namespace NanoHealthLoggingSystem.Context
{
    public class LoggingContext : IdentityDbContext<User>
    {
        public LoggingContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<LoggingMetaData> LoggingMetaData { get; set; }
        public DbSet<Storage> Storage { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
