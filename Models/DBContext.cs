using Microsoft.EntityFrameworkCore;

namespace APITaklimSmart.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        { }

        //Tambah 
    }
}
