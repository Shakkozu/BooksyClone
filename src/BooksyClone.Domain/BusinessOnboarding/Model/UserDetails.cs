namespace BooksyClone.Domain.BusinessOnboarding.Model;

internal class UserDetails
{
    public Guid Guid { get; set; }
    public string FullName { get; set; }

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

    public string IdNumber { get; set; }

    // Validation logic for Email and PhoneNumber
    private bool IsValidEmail(string email) => !string.IsNullOrEmpty(email) && email.Contains("@");
    private bool IsValidPhoneNumber(string phoneNumber) => !string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length == 9;
}