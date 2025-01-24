namespace BooksyClone.Contract.BusinessManagement;

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

public record RegisterEmployeeAccountUsingNewEmployeeTokenRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassowrd { get; set; }
    public string PhoneNumber { get; set; }
    public string Token { get; set; }
}

public record RegistrationToken
{
    private const int _tokenLength = 6;
    private const int _tokenTTLInHours = 72;
    public string Token { get; set; }
    public DateTime ValidTo { get; set; }

    public static RegistrationToken CreateNew()
    {
        var token = Guid.NewGuid().ToString()[.._tokenLength];
        return new RegistrationToken(
            DateTime.UtcNow.AddHours(_tokenTTLInHours),
            token);
    }
    public RegistrationToken()
    {
    }

    private RegistrationToken(DateTime validTo, string token)
    {
        ValidTo = validTo;
        Token = token;
    }
}