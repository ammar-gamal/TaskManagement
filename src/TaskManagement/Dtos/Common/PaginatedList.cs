namespace TaskManagement.Dtos.Common;

public class PaginatedList<T>
{
    public List<T> PageItems { get; set; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public int PageIndex { get; }
    public bool HasNext => TotalPages > PageIndex;
    public bool HasPrevious => PageIndex > 1;
    public PaginatedList(List<T> pageItems, int totalPages, int totalCount, int pageIndex)
    {
        PageItems = pageItems;
        TotalPages = totalPages;
        TotalCount = totalCount;
        PageIndex = pageIndex;
    }
}