using ChallengeCounter.Api.Database.Models;

using Microsoft.EntityFrameworkCore;

namespace ChallengeCounter.Api.Database;

public class WorkoutLogDbContext(DbContextOptions<WorkoutLogDbContext> options) : DbContext(options)
{
    public DbSet<WorkoutLog> WorkoutLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkoutLog>(entity =>
        {
            entity.ToTable("workout_log");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId);
            entity.Property(e => e.ExerciseDate).IsRequired();
            entity.Property(e => e.Pushups).IsRequired();
            entity.Property(e => e.Squats).IsRequired();
            entity.Property(e => e.Abs).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });
    }
}