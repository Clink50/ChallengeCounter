namespace ChallengeCounter.Api.Database.Models;

public record WorkoutLog(int Id, Guid? UserId, DateTime ExerciseDate, int Pushups, int Squats, int Abs)
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}