using Application01_GitAction_Deployment.Models;
using Microsoft.EntityFrameworkCore;

namespace Application01_GitAction_Deployment.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Employee> Employees => Set<Employee>();
    }
}
