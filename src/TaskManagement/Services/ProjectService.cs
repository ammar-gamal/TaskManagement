using Microsoft.EntityFrameworkCore;
using TaskManagement.Dtos.Common;
using TaskManagement.Dtos.Project;
using TaskManagement.ExtensionMethods;
using TaskManagement.ExtensionMethods.Mapping;
using TaskManagement.Persistence.Interfaces;
using TaskManagement.Services.Interfaces;
using TaskManagement.Utilites;

namespace TaskManagement.Services;

public class ProjectService : IProjectService
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<ProjectService> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<ProjectDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving project {ProjectId} for user {UserId}.", id, _currentUser.Id);

        var dto = await _context.Projects
            .GetProjectsForUser(_currentUser.Id)
            .Where(p => p.Id == id)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
        {
            _logger.LogWarning("Project {ProjectId} was not found for user {UserId}.", id, _currentUser.Id);
            return Error.NotFound($"Project {id} was not found.");
        }

        return dto;
    }

    public async Task<Result<PaginatedList<ProjectDto>>> ListAsync(PaginationQueryParameters query, CancellationToken ct = default)
    {
        _logger.LogDebug(
            "Listing projects for user {UserId}. Page: {Page}, Size: {Limit}.",
            _currentUser.Id,
            query.PageIndex,
            query.Limit);

        var projects = await _context.Projects
            .GetProjectsForUser(_currentUser.Id)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
            })
            .OrderBy(p => p.Id)
            .ToPaginatedListAsync(query, ct);

        return projects;
    }

    public async Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating project '{ProjectName}' for user {UserId}.", dto.Name, _currentUser.Id);

        var nameExists = await _context.Projects.AnyAsync(p => p.Name == dto.Name, ct);

        if (nameExists)
        {
            _logger.LogWarning("Project creation failed. Project name '{ProjectName}' already exists.", dto.Name);
            return Error.Conflict($"A project named '{dto.Name}' already exists.");
        }

        var project = dto.ToEntity(_currentUser.Id);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Project {ProjectId} created successfully for user {UserId}.", project.Id, _currentUser.Id);
        return project.ToDto();
    }

    public async Task<Result<ProjectDto>> UpdateAsync(int id, UpdateProjectDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating project {ProjectId} for user {UserId}.", id, _currentUser.Id);

        var project = await _context.Projects
            .GetProjectsForUser(_currentUser.Id)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        // Return NotFound instead of Forbidden in case the user is no longer the owner of the project,
        // to prevent resource enumeration.
        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} was not found for user {UserId}.", id, _currentUser.Id);
            return Error.NotFound($"Project {id} was not found.");
        }

        var nameTaken = await _context.Projects.AnyAsync(p => p.Id != id && p.Name == dto.Name, ct);

        if (nameTaken)
        {
            _logger.LogWarning("Project update failed. Project name '{ProjectName}' already exists.", dto.Name);
            return Error.Conflict($"A project named '{dto.Name}' already exists.");
        }

        project.Name = dto.Name;
        project.Description = dto.Description;
        _context.Projects.Update(project);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Project {ProjectId} updated successfully.", id);
        return project.ToDto();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting project {ProjectId} for user {UserId}.", id, _currentUser.Id);

        var project = await _context.Projects
            .GetProjectsForUser(_currentUser.Id)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        // Return NotFound instead of Forbidden to prevent resource enumeration.
        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} was not found for user {UserId}.", id, _currentUser.Id);
            return Error.NotFound($"Project {id} was not found.");
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Project {ProjectId} deleted successfully.", id);
        return Result.Ok();
    }
}