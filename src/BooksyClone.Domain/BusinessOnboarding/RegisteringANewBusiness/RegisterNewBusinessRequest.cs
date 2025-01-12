using Microsoft.AspNetCore.Http;

namespace BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;

public class RegisterNewBusinessRequest
{
    public Guid CorrelationId { get; set; }
    public DateTime Timestamp { get; set; }
    public required string BusinessName { get; set; }  
    public required BusinessType BusinessType { get; set; }  
    public required string BusinessNIP { get; set; }  
    public required string BusinessAddress { get; set; }  
    public required string BusinessPhoneNumber { get; set; }  
    public required string BusinessEmail { get; set; }  
    
    public Guid UserId { get; set; }
    public required string UserFullName { get; set; }  
    public required string UserIdNumber { get; set; }  
    public required string UserEmail { get; set; }  
    public required string UserPhoneNumber { get; set; }  
    
    public IFormFile BusinessProofDocument { get; set; }  
    public IFormFile UserIdentityDocument { get; set; }  


    public bool LegalConsent { get; set; }  
    public required string LegalConsentContent { get; set; }

    internal IEnumerable<ValidationError> GetValidationErrors()
    {
        // todo add validaiton
        return [];
    }
}
