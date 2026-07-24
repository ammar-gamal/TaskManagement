using TaskManagement.Dtos.Common;
using TaskManagement.Dtos.Task;
using TaskManagement.Utilites;

namespace TaskManagement.Services.Interfaces;

public interface ITaskService
{
    Task<Result<TaskDetailsDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<PaginatedList<TaskDetailsDto>>> ListAllAsync(TaskQueryParameters query, CancellationToken ct = default);
    Task<Result<PaginatedList<TaskDetailsDto>>> ListForProjectAsync(int projectId, TaskQueryParameters query, CancellationToken ct = default);
    Task<Result<TaskDto>> CreateAsync(int projectId, CreateTaskDto dto, CancellationToken ct = default);
    Task<Result<TaskDto>> UpdateAsync(int id, UpdateTaskDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, CancellationToken ct = default);
}
