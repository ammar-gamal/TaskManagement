using Microsoft.EntityFrameworkCore;
using TaskManagement.Dtos.Common;
using TaskManagement.Dtos.Task;
using TaskManagement.Entities.Enums;
using TaskManagement.ExtensionMethods;
using TaskManagement.ExtensionMethods.Mapping;
using TaskManagement.Persistence.Interfaces;
using TaskManagement.Services.Interfaces;
using TaskManagement.Utilites;

namespace TaskManagement.Services;

public class TaskService : ITaskService
{
    private readonly IAppDbContext _context;
    private readonly ILogger<TaskService> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly ICurrentUserService _currentUser;

    public TaskService(
        ILogger<TaskService> logger,
        TimeProvider timeProvider,
        IAppDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _logger = logger;
        _timeProvider = timeProvider;
        _currentUser = currentUser;
    }

    public async Task<Result<TaskDetailsDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving task {TaskId} for user {UserId}.", id, _currentUser.Id);

        var dto = await _context.Tasks
            .GetTasksForUser(_currentUser.Id)
            .Where(t => t.Id == id)
            .Select(t => new TaskDetailsDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
        {
            _logger.LogWarning("Task {TaskId} was not found for user {UserId}.", id, _currentUser.Id);
            return Error.NotFound($"Task {id} was not found.");
        }

        return dto;
    }

    public async Task<Result<PaginatedList<TaskDetailsDto>>> ListAllAsync(TaskQueryParameters query,
        CancellationToken ct = default)
    {
        _logger.LogDebug(
            "Listing tasks for user {UserId} - Search: {Search}, Status: {Status}, Priority: {Priority}, SortBy: {SortBy}, SortDirection: {SortDirection}.",
            _currentUser.Id,
            query.Q,
            query.Status,
            query.Priority,
            query.SortBy,
            query.SortDir);

        var tasks = await _context.Tasks
            .GetTasksForUser(_currentUser.Id)
            .ApplyFiltersAndSorting(query)
            .Select(t => new TaskDetailsDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
            })
            .ToPaginatedListAsync(query, ct);

        return tasks;
    }

    public async Task<Result<PaginatedList<TaskDetailsDto>>> ListForProjectAsync(
        int projectId,
        TaskQueryParameters query,
        CancellationToken ct = default)
    {
        _logger.LogDebug(
            "Listing tasks for project {ProjectId} and user {UserId} - Search: {Search}, Status: {Status}, Priority: {Priority}, SortBy: {SortBy}, SortDirection: {SortDirection}.",
            projectId,
            _currentUser.Id,
            query.Q,
            query.Status,
            query.Priority,
            query.SortBy,
            query.SortDir);

        var projectExists = await _context.Projects
            .GetProjectsForUser(_currentUser.Id)
            .AnyAsync(e => e.Id == projectId, ct);

        if (!projectExists)
        {
            _logger.LogWarning(
                 "Project {ProjectId} was not found for user {UserId}.",
                 projectId,
                 _currentUser.Id);
            return Error.NotFound($"Project {projectId} was not found.");
        }
        var tasks = await _context.Tasks
            .Where(e => e.ProjectId == projectId)
            .ApplyFiltersAndSorting(query)
            .Select(t => new TaskDetailsDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
            })
            .ToPaginatedListAsync(query, ct);

        return tasks;
    }

    public async Task<Result<TaskDto>> CreateAsync(int projectId, CreateTaskDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating task in project {ProjectId} for user {UserId}.", projectId, _currentUser.Id);

        var projectExists = await _context.Projects
            .GetProjectsForUser(_currentUser.Id)
            .AnyAsync(p => p.Id == projectId, ct);

        if (!projectExists)
        {
            _logger.LogWarning(
                "Task creation failed. Project {ProjectId} was not found for user {UserId}.",
                projectId,
                _currentUser.Id);
            return Error.NotFound($"Project {projectId} was not found.");
        }

        var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        if (dto.DueDate.HasValue && dto.DueDate.Value < today)
        {
            _logger.LogWarning(
                "Task creation failed. Invalid due date {DueDate} for project {ProjectId}.",
                dto.DueDate,
                projectId);
            return Error.BadRequest("Due date cannot be in the past.");
        }

        var task = dto.ToEntity(projectId);

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Task {TaskId} created successfully in project {ProjectId}.", task.Id, projectId);
        return task.ToDto();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting task {TaskId} for user {UserId}.", id, _currentUser.Id);

        var task = await _context.Tasks
            .GetTasksForUser(_currentUser.Id)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} was not found for user {UserId}.", id, _currentUser.Id);
            return Error.NotFound($"Task {id} was not found.");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Task {TaskId} deleted successfully.", id);
        return Result.Ok();
    }

    public async Task<Result<TaskDto>> UpdateAsync(int id, UpdateTaskDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating task {TaskId} for user {UserId}.", id, _currentUser.Id);

        var task = await _context.Tasks
            .GetTasksForUser(_currentUser.Id)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        if (task is null)
        {
            _logger.LogWarning(
                "Task update failed. Task {TaskId} was not found for user {UserId}.",
                id,
                _currentUser.Id);
            return Error.NotFound($"Task {id} was not found.");
        }

        var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        if (dto.DueDate.HasValue && dto.DueDate.Value < today)
        {
            _logger.LogWarning(
                "Task update failed. Invalid due date {DueDate} for task {TaskId}.",
                dto.DueDate,
                id);
            return Error.BadRequest("Due date cannot be in the past.");
        }

        if (task.Status == Status.Done && dto.Status == Status.Todo)
        {
            _logger.LogWarning(
                "Unusual status transition for task {TaskId}: {FromStatus} -> {ToStatus}",
                id,
                task.Status,
                dto.Status);
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Task {TaskId} updated successfully by User {UserId}", id, _currentUser.Id);
        return task.ToDto();
    }
}