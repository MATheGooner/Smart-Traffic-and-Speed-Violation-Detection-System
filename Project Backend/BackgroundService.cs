using Microsoft.EntityFrameworkCore;
using TrafficControlSystem.Context;

namespace TrafficControlSystem;

public class TBackgroundService : BackgroundService
{
    IServiceScopeFactory _serviceScopeFactory;
    public TBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    protected async override Task ExecuteAsync(CancellationToken token)
    {
        using var vscope = _serviceScopeFactory.CreateScope();
        var context = vscope.ServiceProvider.GetRequiredService<TrafficControlSystemContext>();
        // await context.Database.MigrateAsync();
        await Task.CompletedTask;
    }
}
