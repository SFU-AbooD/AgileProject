using Microsoft.EntityFrameworkCore;
using AgileProject.Models;
using System.Xml.Linq;

namespace AgileProject.Migrations
{
    public class _dbcontext : DbContext
    {
        public _dbcontext(DbContextOptions opt) : base(opt) { }
        public DbSet<Tickets>? Tickets { get; set; }
        public DbSet<User_>? User { get; set; }
        public DbSet<comments>? comments { get; set; }
        public DbSet<files>? files { get; set; }
        public DbSet<updates>? updates { get; set; }
    }
}
