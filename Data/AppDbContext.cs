using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Models;

namespace TaskAppAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<User> Users { get; set; } // 🔥 WAJIB
    }
}