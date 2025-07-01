using ChallengeCounter.Api.Database;

using Microsoft.EntityFrameworkCore;

namespace ChallengeCounter.Api;

public class MigrationHostedService<TContext>(IServiceProvider serviceProvider) : IHostedService where TContext : WorkoutLogDbContext
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
