using TaskManagement.Dtos.Common;
using TaskManagement.Entities.Enums;

namespace TaskManagement.Dtos.Task;

public class TaskQueryParameters : PaginationQueryParameters
{
    public Status? Status { get; set; }
    public Priority? Priority { get; set; }
    public DateOnly? DueDateFrom { get; set; }
    public DateOnly? DueDateTo { get; set; }
    public string? Q { get; set; }
    public TaskSortField? SortBy { get; set; }
    public SortDirection? SortDir { get; set; }
}
public enum TaskSortField
{
    DueDate,
    Priority,
    CreatedAt
}
public enum SortDirection
{
    Asc,
    Desc
}