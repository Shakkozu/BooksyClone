using System.Runtime.Serialization;

namespace BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;


public record ValidationError(string PropertyName, string ErrorMessage);