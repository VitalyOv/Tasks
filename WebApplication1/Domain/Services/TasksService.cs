using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services;

public class TasksService: ITasksService
{
    private readonly DatabaseContext.DatabaseContext _databaseContext;
    
    public TasksService(DatabaseContext.DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<Guid> AddTask()
    {
        var entityEntry = _databaseContext.Tasks.Add(new Entities.Task());
        await _databaseContext.SaveChangesAsync();

        return entityEntry.Entity.Id;
    }

    public async Task<Status?> GetTaskStatus(Guid id)
    {
        var item = await _databaseContext.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

        return item?.Status;
    }
}
