using ChallengeCounter.Api.Database;

using Microsoft.EntityFrameworkCore;

namespace ChallengeCounter.Api;

public class MigrationHostedService<TContext>(IServiceProvider serviceProvider,ILogger<MigrationHostedService<TContext>> logger) : IHostedService where TContext : WorkoutLogDbContext
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting database initialization.");

            using var scope = _serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
