namespace ChallengeCounter.Api.Models;

public record WorkoutLogDto(string UserId, DateTime ExerciseDate, int Pushups, int Squats, int Abs);
