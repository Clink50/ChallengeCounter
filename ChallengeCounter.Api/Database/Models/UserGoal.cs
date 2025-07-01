namespace ChallengeCounter.Api.Database.Models;

public class UserGoal
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int PushupsGoal { get; set; }
    public int SquatsGoal { get; set; }
    public int AbsGoal { get; set; }
}
