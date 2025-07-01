using ChallengeCounter.Api.Database.Models;

using Microsoft.EntityFrameworkCore;

namespace ChallengeCounter.Api.Database;

public class WorkoutLogDbContext(DbContextOptions<WorkoutLogDbContext> options) : DbContext(options)
{
    public DbSet<WorkoutLog> WorkoutLogs { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }

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

        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.ToTable("user_goal");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Year).IsRequired();
            entity.Property(e => e.Month).IsRequired();
            entity.Property(e => e.PushupsGoal).IsRequired();
            entity.Property(e => e.SquatsGoal).IsRequired();
            entity.Property(e => e.AbsGoal).IsRequired();
            entity.HasIndex(e => new { e.UserId, e.Year, e.Month }).IsUnique();
        });
    }
}