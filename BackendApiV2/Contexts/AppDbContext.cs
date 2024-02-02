using BackendApiV2.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendApiV2.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Trainee> Trainees { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trainee>().ToTable("trainees");
            modelBuilder.Entity<User>().ToTable("users");
        }
    }
}
