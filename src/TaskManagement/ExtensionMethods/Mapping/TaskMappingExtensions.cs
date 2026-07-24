using TaskManagement.Dtos.Task;
using TaskManagement.Entities;
using TaskManagement.Entities.Enums;

namespace TaskManagement.ExtensionMethods.Mapping;

public static class TaskMappingExtensions
{
    public static TaskItem ToEntity(this CreateTaskDto dto, int projectId) =>
    new()
    {
        ProjectId = projectId,
        Title = dto.Title,
        Description = dto.Description,
        Status = dto.Status ?? Status.Todo,
        Priority = dto.Priority ?? Priority.Medium,
        DueDate = dto.DueDate
    };
    public static TaskDto ToDto(this TaskItem entity) =>
    new()
    {
        Id = entity.Id,
        ProjectId = entity.ProjectId,
        Title = entity.Title,
        Description = entity.Description,
        Status = entity.Status,
        Priority = entity.Priority,
        DueDate = entity.DueDate,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
