using BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;

namespace BooksyClone.Domain.BusinessOnboarding.Model;

public class BusinessDetails
{
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public BusinessType Type { get; set; }

    private string _nip;
    public string Nip
    {
        get => _nip;
        set
        {
            if (!IsValidNip(value))
                throw new ArgumentException("Invalid NIP format.");
            _nip = value;
        }
    }

    private string _phoneNumber;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            if (!IsValidPhoneNumber(value))
                throw new ArgumentException("Invalid phone number format.");
            _phoneNumber = value;
        }
    }

    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            if (!IsValidEmail(value))
                throw new ArgumentException("Invalid email format.");
            _email = value;
        }
    }

    public string Address { get; set; }

    // Validation methods for NIP, PhoneNumber, and Email
    private bool IsValidNip(string nip)
    {
        // Implement proper NIP validation logic
        return !string.IsNullOrEmpty(nip) && nip.Length == 10; // Simple validation example
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        // Simple phone number validation example
        return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length == 9;
    }

    private bool IsValidEmail(string email)
    {
        return !string.IsNullOrEmpty(email) && email.Contains("@");
    }
}
