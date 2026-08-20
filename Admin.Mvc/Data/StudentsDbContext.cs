using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Admin.Mvc.Models;
using School.Shared.Models;

namespace Admin.Mvc.Data
{
    public class StudentsDbContext : IdentityDbContext<ApplicationUser>
    {
        public StudentsDbContext(DbContextOptions<StudentsDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
    }
}
