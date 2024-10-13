using System.Diagnostics.CodeAnalysis;

namespace BooksyClone.Contract.Shared;
public class PagedListResponse<T> where T : class
{
    public IReadOnlyList<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public PagedListResponse(IEnumerable<T> items, int page, int pageSize, int totalCount)
    {
        Items = items.ToList();
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public PagedListResponse() //required for deserialization
    {
        
    }
}


public record Paging(int Page, int PageSize);
