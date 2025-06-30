namespace ChallengeCounter.Api.Database.Models;

public class WorkoutLog
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public DateTime ExerciseDate { get; set; }
    public int Pushups { get; set; }
    public int Squats { get; set; }
    public int Abs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}