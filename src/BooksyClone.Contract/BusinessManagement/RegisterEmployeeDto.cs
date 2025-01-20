namespace BooksyClone.Contract.BusinessManagement;

public class RegisterEmployeeDto
{
    
}

public record RegisterNewEmployeeRequest
{
    public Guid EmployeeId { get; set; }
    public Guid BusinessUnitId { get; set; }
    
    public DateTime ValidFrom { get; set; }
    
    public DateTime ValidTo { get; set; }
    
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
}