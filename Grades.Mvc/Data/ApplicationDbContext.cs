using Microsoft.EntityFrameworkCore;
using Grades.Mvc.Models;

namespace Grades.Mvc.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Grade> Grades => Set<Grade>();
}

