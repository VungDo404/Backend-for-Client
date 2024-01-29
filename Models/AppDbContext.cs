using Microsoft.EntityFrameworkCore;

namespace Project.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Document>? Documents { get; set; }
    }
}
