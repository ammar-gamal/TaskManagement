using TaskManagement.Dtos.Project;
using TaskManagement.Entities;

namespace TaskManagement.ExtensionMethods.Mapping;

public static class ProjectMappingExtensions
{
    public static Project ToEntity(this CreateProjectDto dto, int userId) =>
    new()
    {
        Name = dto.Name,
        Description = dto.Description,
        UserId = userId
    };
    public static ProjectDto ToDto(this Project entity) =>
    new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        Description = entity.Description,
        Name = entity.Name,
        UpdatedAt = entity.UpdatedAt
    };
}
