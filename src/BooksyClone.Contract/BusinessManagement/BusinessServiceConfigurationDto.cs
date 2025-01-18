using BooksyClone.Contract.Shared;

namespace BooksyClone.Contract.BusinessManagement;

public record BusinessServiceConfigurationDto
{
    public Guid BusinessUnitId { get; set; }
    public List<OfferedServiceDto> OfferedServices { get; set; }
}

public record OfferedServiceDto
{
    public Guid Guid { get; set; }

    public Guid EmployeeId { get; set; }
    public string Name { get; set; }
    public string MarkdownDescription { get; set; }
    public List<long> GenericServiceVariantsIds { get; set; }
    public TimeSpan Duration { get; set; }
    public Money Price { get; set; }
    public int Order { get; set; }
    public long CategoryId { get; set; }
}

