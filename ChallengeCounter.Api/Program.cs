using ChallengeCounter.Api;
using ChallengeCounter.Api.Database;
using ChallengeCounter.Api.Database.Models;
using ChallengeCounter.Api.Helpers;
using ChallengeCounter.Api.Models;

using Microsoft.EntityFrameworkCore;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("challengecounter");
builder.Services.AddDbContextPool<WorkoutLogDbContext>(dbContextOptionsBuilder =>
    dbContextOptionsBuilder.UseNpgsql(connectionString, options =>
        options.MigrationsHistoryTable("migration_history"))
    .UseSnakeCaseNamingConvention());
builder.EnrichNpgsqlDbContext<WorkoutLogDbContext>();

builder.Services.AddHostedService<MigrationHostedService<WorkoutLogDbContext>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// POST /api/log - Log a set
app.MapPost("/api/log", async (WorkoutLogDto dto, string? timezone, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.UserId))
        return Results.BadRequest("Missing userId");
    var zone = Helpers.GetZone(timezone);
    var localDate = LocalDateTime.FromDateTime(dto.ExerciseDate);
    var zoned = zone.AtStrictly(localDate); // treat ExerciseDate as local in user's zone
    var log = new WorkoutLog
    {
        UserId = dto.UserId.ToLower(),
        ExerciseDate = zoned.ToDateTimeUtc(),
        Pushups = dto.Pushups,
        Squats = dto.Squats,
        Abs = dto.Abs,
        CreatedAt = DateTime.UtcNow
    };
    db.WorkoutLogs.Add(log);
    await db.SaveChangesAsync();
    return Results.Created($"/api/log/{log.Id}", log);
});

// GET /api/today - Get today's totals for a user
app.MapGet("/api/today", async (string userId, string? timezone, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(userId))
        return Results.BadRequest("Missing userId");
    var zone = Helpers.GetZone(timezone);
    var now = SystemClock.Instance.GetCurrentInstant().InZone(zone);
    var start = now.Date.AtStartOfDayInZone(zone).ToInstant().ToDateTimeUtc();
    var end = now.Date.PlusDays(1).AtStartOfDayInZone(zone).ToInstant().ToDateTimeUtc();
    var logs = await db.WorkoutLogs
        .Where(x => x.ExerciseDate >= start && x.ExerciseDate < end && x.UserId.ToLower() == userId.ToLower())
        .ToListAsync();
    return Results.Ok(new
    {
        Pushups = logs.Sum(x => x.Pushups),
        Squats = logs.Sum(x => x.Squats),
        Abs = logs.Sum(x => x.Abs),
        Sets = logs.Count
    });
});

// GET /api/history - Get previous days' totals for a user
app.MapGet("/api/history", async (string userId, string? timezone, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(userId))
        return Results.BadRequest("Missing userId");
    var zone = Helpers.GetZone(timezone);
    var logs = await db.WorkoutLogs
        .Where(x => x.UserId.ToLower() == userId.ToLower())
        .ToListAsync();
    var history = logs
        .GroupBy(x => LocalDateTime.FromDateTime(x.ExerciseDate).InZoneLeniently(DateTimeZone.Utc).WithZone(zone).Date)
        .OrderByDescending(g => g.Key)
        .Select(g => new
        {
            Date = g.Key.ToDateOnly(),
            Pushups = g.Sum(x => x.Pushups),
            Squats = g.Sum(x => x.Squats),
            Abs = g.Sum(x => x.Abs),
            Sets = g.Count()
        })
        .ToList();
    return Results.Ok(history);
});

// GET /api/monthly - Get monthly totals for a user
app.MapGet("/api/monthly", async (string userId, int? year, int? month, string? timezone, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(userId))
        return Results.BadRequest("Missing userId");
    var zone = Helpers.GetZone(timezone);
    var now = SystemClock.Instance.GetCurrentInstant().InZone(zone);
    int y = year ?? now.Year;
    int m = month ?? now.Month;
    var start = new LocalDate(y, m, 1).AtStartOfDayInZone(zone).ToInstant().ToDateTimeUtc();
    var end = (m == 12 ? new LocalDate(y + 1, 1, 1) : new LocalDate(y, m + 1, 1)).AtStartOfDayInZone(zone).ToInstant().ToDateTimeUtc();
    var logs = await db.WorkoutLogs
        .Where(x => x.UserId.ToLower() == userId.ToLower() && x.ExerciseDate >= start && x.ExerciseDate < end)
        .ToListAsync();
    return Results.Ok(new
    {
        Year = y,
        Month = m,
        Pushups = logs.Sum(x => x.Pushups),
        Squats = logs.Sum(x => x.Squats),
        Abs = logs.Sum(x => x.Abs),
        Sets = logs.Count
    });
});

// GET /api/leaderboard - Get monthly totals for all users
app.MapGet("/api/leaderboard", async (int? year, int? month, string? timezone, WorkoutLogDbContext db) =>
{
    var zone = Helpers.GetZone(timezone);
    var now = SystemClock.Instance.GetCurrentInstant().InZone(zone);
    int y = year ?? now.Year;
    int m = month ?? now.Month;
    var start = new LocalDate(y, m, 1).AtStartOfDayInZone(zone).ToInstant().ToDateTimeUtc();
    var end = (m == 12 ? new LocalDate(y + 1, 1, 1) : new LocalDate(y, m + 1, 1)).AtStartOfDayInZone(zone).ToInstant().ToDateTimeUtc();
    var leaderboard = await db.WorkoutLogs
        .Where(x => x.ExerciseDate >= start && x.ExerciseDate < end)
        .GroupBy(x => x.UserId.ToLower())
        .Select(g => new
        {
            UserId = g.Key.ToLower(),
            Pushups = g.Sum(x => x.Pushups),
            Squats = g.Sum(x => x.Squats),
            Abs = g.Sum(x => x.Abs),
            Sets = g.Count()
        })
        .OrderByDescending(x => x.Pushups + x.Squats + x.Abs)
        .ToListAsync();
    return Results.Ok(leaderboard);
});

// POST /api/goal - Set monthly goal for a user
app.MapPost("/api/goal", async (UserGoalDto dto, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.UserId))
        return Results.BadRequest("Missing userId");
    var existing = await db.UserGoals.FirstOrDefaultAsync(g => g.UserId.ToLower() == dto.UserId.ToLower() && g.Year == dto.Year && g.Month == dto.Month);
    if (existing != null)
    {
        existing.PushupsGoal = dto.PushupsGoal;
        existing.SquatsGoal = dto.SquatsGoal;
        existing.AbsGoal = dto.AbsGoal;
    }
    else
    {
        db.UserGoals.Add(new UserGoal
        {
            UserId = dto.UserId.ToLower(),
            Year = dto.Year,
            Month = dto.Month,
            PushupsGoal = dto.PushupsGoal,
            SquatsGoal = dto.SquatsGoal,
            AbsGoal = dto.AbsGoal
        });
    }
    await db.SaveChangesAsync();
    return Results.Ok();
});

// GET /api/goal - Get monthly goal for a user
app.MapGet("/api/goal", async (string userId, int? year, int? month, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(userId))
        return Results.BadRequest("Missing userId");
    var now = SystemClock.Instance.GetCurrentInstant().InZone(NodaTime.DateTimeZone.Utc);
    int y = year ?? now.Year;
    int m = month ?? now.Month;
    var goal = await db.UserGoals.FirstOrDefaultAsync(g => g.UserId.ToLower() == userId.ToLower() && g.Year == y && g.Month == m);
    if (goal == null) return Results.NotFound();
    return Results.Ok(new
    {
        goal.UserId,
        goal.Year,
        goal.Month,
        goal.PushupsGoal,
        goal.SquatsGoal,
        goal.AbsGoal
    });
});

app.Run();
