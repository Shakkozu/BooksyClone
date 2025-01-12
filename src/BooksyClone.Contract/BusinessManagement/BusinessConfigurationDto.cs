using BooksyClone.Contract.Shared;

namespace BooksyClone.Contract.BusinessManagement;

public record BusinessConfigurationDto
{
    public Guid BusinessUnitId { get; set; }
    public Guid EmployeeId { get; set; }
    public List<OfferedServiceDto> OfferedServices { get; set; }
}

public record OfferedServiceDto
{
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public string MarkdownDescription { get; set; }
    public List<ulong> GenericServiceVariantsIds { get; set; }
    public TimeSpan Duration { get; set; }
    public Money Price { get; set; }
    public int Order { get; set; }
    public List<Guid> EmployeesOfferingService { get; set; }
    public ulong CategoryId { get; set; }
}