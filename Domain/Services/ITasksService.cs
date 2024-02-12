using Domain.Entities;

namespace Domain.Services;

public interface ITasksService
{
    Task<Guid> AddTask();

    Task<Status?> GetTaskStatus(Guid id);
}
