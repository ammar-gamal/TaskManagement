using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Dtos.Common;
using TaskManagement.Dtos.Project;
using TaskManagement.Dtos.Task;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers;

[Authorize]
[ApiController]
[Produces("application/json")]
[Route("api/projects")]
public class ProjectsController : AppController
{
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;

    public ProjectsController(IProjectService projectService, ITaskService taskService)
    {
        _projectService = projectService;
        _taskService = taskService;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _projectService.GetByIdAsync(id, ct);
        if (!result.IsSuccess)
            return HandleError(result);

        return Ok(result.Data);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] PaginationQueryParameters query, CancellationToken ct)
    {
        var result = await _projectService.ListAsync(query, ct);
        return Ok(result.Data);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto, CancellationToken ct)
    {
        var result = await _projectService.CreateAsync(dto, ct);
        if (!result.IsSuccess)
            return HandleError(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
    }


    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto, CancellationToken ct)
    {
        var result = await _projectService.UpdateAsync(id, dto, ct);
        if (!result.IsSuccess)
            return HandleError(result);

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _projectService.DeleteAsync(id, ct);
        if (!result.IsSuccess)
            return HandleError(result);
        return NoContent();
    }

    [HttpPost("{id:int}/tasks")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask(int id, [FromBody] CreateTaskDto dto, CancellationToken ct)
    {
        var result = await _taskService.CreateAsync(id, dto, ct);
        if (!result.IsSuccess)
            return HandleError(result);
        return CreatedAtAction("GetById", "Tasks", new { id = result.Data.Id }, result.Data);
    }

    [HttpGet("{id:int}/tasks")]
    [ProducesResponseType(typeof(PaginatedList<TaskDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListTasks(int id, [FromQuery] TaskQueryParameters query, CancellationToken ct)
    {
        var result = await _taskService.ListForProjectAsync(id, query, ct);
        if (!result.IsSuccess)
            return HandleError(result);
        return Ok(result.Data);
    }
}
