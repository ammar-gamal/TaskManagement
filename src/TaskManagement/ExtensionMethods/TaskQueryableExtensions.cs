using Microsoft.EntityFrameworkCore;
using TaskManagement.Dtos.Task;
using TaskManagement.Entities;

namespace TaskManagement.ExtensionMethods;

public static class TaskQueryableExtensions
{
    public static IQueryable<TaskItem> GetTasksForUser(this IQueryable<TaskItem> tasks, int userId)
        => tasks.Where(e => e.Project.UserId == userId);


    public static IQueryable<TaskItem> ApplyFiltersAndSorting(this IQueryable<TaskItem> tasks, TaskQueryParameters query)
    {
        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var searchTerm = query.Q.Trim();
            //sql server is case insensitive

            tasks = tasks.Where(t =>
                EF.Functions.Like(t.Title, $"%{searchTerm}%") ||
                (t.Description != null && EF.Functions.Like(t.Description, $"%{searchTerm}%"))
            );
        }
        if (query.Status.HasValue)
        {
            tasks = tasks.Where(t => t.Status == query.Status.Value);
        }
        if (query.Priority.HasValue)
        {
            tasks = tasks.Where(t => t.Priority == query.Priority.Value);
        }
        if (query.DueDateFrom.HasValue)
        {
            tasks = tasks.Where(t => t.DueDate >= query.DueDateFrom.Value);
        }
        if (query.DueDateTo.HasValue)
        {
            tasks = tasks.Where(t => t.DueDate <= query.DueDateTo.Value);
        }


        var sortDir = query.SortDir is null ? SortDirection.Desc
                                                      : query.SortDir;


        var sortBy = query.SortBy is null ? TaskSortField.CreatedAt
                                                    : query.SortBy;

        tasks = sortBy switch
        {
            TaskSortField.CreatedAt => sortDir is SortDirection.Asc ? tasks.OrderBy(t => t.CreatedAt).ThenBy(t => t.Id)
                                                                    : tasks.OrderByDescending(t => t.CreatedAt).ThenBy(t => t.Id),
            TaskSortField.DueDate => sortDir is SortDirection.Asc ? tasks.OrderBy(t => t.DueDate).ThenBy(t => t.Id)
                                                                  : tasks.OrderByDescending(t => t.DueDate).ThenBy(t => t.Id),
            TaskSortField.Priority => sortDir is SortDirection.Asc ? tasks.OrderBy(t => t.Priority).ThenBy(t => t.Id)
                                                                  : tasks.OrderByDescending(t => t.Priority).ThenBy(t => t.Id),
            _ => tasks
        };

        return tasks;
    }
}
