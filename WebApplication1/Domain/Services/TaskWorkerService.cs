using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Domain.Services;

public class TaskWorkerService: IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TaskWorkerService> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource = new ();
    private const int OrdersPerTime = 100;
    private readonly TimeSpan DelayTime = TimeSpan.FromSeconds(3);
    private Task? doWorkTask;
    
    public TaskWorkerService(IServiceScopeFactory scopeFactory, ILogger<TaskWorkerService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        doWorkTask = DoWork();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();

        if (doWorkTask is not null)
        {
            await doWorkTask.WaitAsync(cancellationToken);
        }
    }

    private async Task DoWork()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();

                var databaseContext = scope.ServiceProvider.GetService<DatabaseContext.DatabaseContext>();

                var dateTimeFrameForRunningTasks = DateTime.UtcNow.AddMinutes(-2);

                var orders = await databaseContext!.Tasks
                    .Where(t => t.Status == Status.created || (t.Status == Status.running && t.UpdatedTime <= dateTimeFrameForRunningTasks))
                    .Take(OrdersPerTime)
                    .ToListAsync(_cancellationTokenSource.Token);

                if (!orders.Any())
                {
                    await Task.Delay(DelayTime, _cancellationTokenSource.Token);
                }

                foreach (var order in orders)
                {
                    if (order.Status == Status.created)
                    {
                        order.Status = Status.running;
                        order.UpdatedTime = DateTime.UtcNow;
                        continue;
                    }

                    order.Status = Status.finished;
                    order.UpdatedTime = DateTime.UtcNow;
                }

                await databaseContext.SaveChangesAsync(_cancellationTokenSource.Token);
            }
            catch when (_cancellationTokenSource.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                await Task.Delay(DelayTime, _cancellationTokenSource.Token);
            }
        }
    }
}
