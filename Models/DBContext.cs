using Microsoft.EntityFrameworkCore;

namespace APITaklimSmart.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Lokasi> Lokasis { get; set; }
    }
}
