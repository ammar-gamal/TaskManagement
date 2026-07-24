using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Dtos.Common;
using TaskManagement.Dtos.Task;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]

public class TasksController : AppController
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TaskDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] TaskQueryParameters query, CancellationToken ct)
    {
        var result = await _taskService.ListAllAsync(query, ct);

        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _taskService.GetByIdAsync(id, ct);
        if (!result.IsSuccess)
            return HandleError(result);
        return Ok(result.Data);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto, CancellationToken ct)
    {
        var result = await _taskService.UpdateAsync(id, dto, ct);
        if (!result.IsSuccess)
            return HandleError(result);

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _taskService.DeleteAsync(id, ct);
        if (!result.IsSuccess)
            return HandleError(result);

        return NoContent();
    }
}
