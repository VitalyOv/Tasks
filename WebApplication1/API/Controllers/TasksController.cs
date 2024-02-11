using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/task")]
public class TasksController: ControllerBase
{
    private readonly ITasksService _tasksService;
    
    public TasksController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddTask()
    {
        var id = await _tasksService.AddTask();
        return CreatedAtRoute(nameof(GetTaskStatus), new { id }, id);
    }

    [HttpGet("{id}", Name = nameof(GetTaskStatus))]
    public async Task<IActionResult> GetTaskStatus(Guid id)
    {
        var status = await _tasksService.GetTaskStatus(id);

        if (status is null)
        {
            return NotFound();
        }

        return Ok(status);
    }
}
