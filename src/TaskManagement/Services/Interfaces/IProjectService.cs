using TaskManagement.Dtos.Common;
using TaskManagement.Dtos.Project;
using TaskManagement.Utilites;

namespace TaskManagement.Services.Interfaces;

public interface IProjectService
{
    Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto, CancellationToken ct = default);
    Task<Result<PaginatedList<ProjectDto>>> ListAsync(PaginationQueryParameters query, CancellationToken ct = default);
    Task<Result<ProjectDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<ProjectDto>> UpdateAsync(int id, UpdateProjectDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, CancellationToken ct = default);
}
