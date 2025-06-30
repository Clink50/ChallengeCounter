using ChallengeCounter.Api.Database;
using ChallengeCounter.Api.Database.Models;
using ChallengeCounter.Api.Models;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<WorkoutLogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), config =>
        config.MigrationsHistoryTable("migration_history"))
    .UseSnakeCaseNamingConvention());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// POST /api/log - Log a set
app.MapPost("/api/log", async (WorkoutLogDto dto, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.UserId))
        return Results.BadRequest("Missing userId");
    var log = new WorkoutLog
    {
        UserId = dto.UserId.ToLower(),
        ExerciseDate = dto.ExerciseDate.Date.ToUniversalTime(),
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
app.MapGet("/api/today", async (string userId, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(userId))
        return Results.BadRequest("Missing userId");
    var today = DateTime.UtcNow.Date;
    var logs = await db.WorkoutLogs
        .Where(x => x.ExerciseDate.Date == today && x.UserId.ToLower() == userId.ToLower())
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
app.MapGet("/api/history", async (string userId, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(userId))
        return Results.BadRequest("Missing userId");
    var history = await db.WorkoutLogs
        .Where(x => x.UserId.ToLower() == userId.ToLower())
        .GroupBy(x => x.ExerciseDate.ToUniversalTime())
        .OrderByDescending(g => g.Key)
        .Select(g => new
        {
            Date = g.Key,
            Pushups = g.Sum(x => x.Pushups),
            Squats = g.Sum(x => x.Squats),
            Abs = g.Sum(x => x.Abs),
            Sets = g.Count()
        })
        .ToListAsync();
    return Results.Ok(history);
});

// GET /api/monthly - Get monthly totals for a user
app.MapGet("/api/monthly", async (string userId, int? year, int? month, WorkoutLogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(userId))
        return Results.BadRequest("Missing userId");
    var now = DateTime.UtcNow;
    int y = year ?? now.Year;
    int m = month ?? now.Month;
    var logs = await db.WorkoutLogs
        .Where(x => x.UserId.ToLower() == userId.ToLower() && x.ExerciseDate.Year == y && x.ExerciseDate.Month == m)
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
app.MapGet("/api/leaderboard", async (int? year, int? month, WorkoutLogDbContext db) =>
{
    var now = DateTime.UtcNow;
    int y = year ?? now.Year;
    int m = month ?? now.Month;
    var leaderboard = await db.WorkoutLogs
        .Where(x => x.ExerciseDate.Year == y && x.ExerciseDate.Month == m)
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

app.Run();
