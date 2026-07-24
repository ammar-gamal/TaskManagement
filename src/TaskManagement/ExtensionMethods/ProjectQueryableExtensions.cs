using TaskManagement.Entities;

namespace TaskManagement.ExtensionMethods;

public static class ProjectQueryableExtensions
{
    public static IQueryable<Project> GetProjectsForUser(this IQueryable<Project> projects, int userId)
        => projects.Where(e => e.UserId == userId);
}
