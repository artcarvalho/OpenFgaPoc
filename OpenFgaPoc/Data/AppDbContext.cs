using Microsoft.EntityFrameworkCore;
using OpenFgaPoc.Model;

namespace OpenFgaPoc.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public AppDbContext(DbContextOptions options ) : base( options ) 
        { 

        }
    }
}
