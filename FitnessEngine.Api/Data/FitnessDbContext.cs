using FitnessEngine.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessEngine.Api.Data;

public class FitnessDbContext : DbContext
{
    public FitnessDbContext(DbContextOptions<FitnessDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public DbSet<Workout> Workouts => Set<Workout>();

    public DbSet<RecommendationHistory> RecommendationHistories =>
        Set<RecommendationHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>()
            .Property(x => x.Goal)
            .HasMaxLength(100);

        modelBuilder.Entity<UserProfile>()
            .Property(x => x.FitnessLevel)
            .HasMaxLength(100);

        modelBuilder.Entity<Workout>()
            .Property(x => x.Name)
            .HasMaxLength(150);

        modelBuilder.Entity<Workout>()
            .Property(x => x.Goal)
            .HasMaxLength(100);

        modelBuilder.Entity<Workout>()
            .Property(x => x.FitnessLevel)
            .HasMaxLength(100);

        modelBuilder.Entity<RecommendationHistory>()
            .HasOne(x => x.UserProfile)
            .WithMany()
            .HasForeignKey(x => x.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
