using Microsoft.EntityFrameworkCore;
using FitnessEngine.Api.Models;

namespace FitnessEngine.Api.Repositories
{
    public class FitnessDbContext : DbContext
    {
        public FitnessDbContext(DbContextOptions<FitnessDbContext> options) : base(options) { }

        public DbSet<Workout> Workouts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLog>()
                .HasKey(ul => ul.Id);

            modelBuilder.Entity<UserLog>()
                .HasOne(ul => ul.Workout)
                .WithMany()
                .HasForeignKey(ul => ul.WorkoutId);
        }
    }
}
