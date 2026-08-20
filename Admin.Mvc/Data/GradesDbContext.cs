using Microsoft.EntityFrameworkCore;
using Admin.Mvc.Models;

namespace Admin.Mvc.Data
{
    public class GradesDbContext : DbContext
    {
        public GradesDbContext(DbContextOptions<GradesDbContext> options) : base(options) { }

        public DbSet<Grade> Grades => Set<Grade>();
    }
}
