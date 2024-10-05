using System.Runtime.Serialization;

namespace BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;


public record ValidationError(string PropertyName, string ErrorMessage);


[Serializable]
public class CommandValidationException : Exception
{
    public CommandValidationException()
    {
    }

    public CommandValidationException(string? message) : base(message)
    {
    }

    public CommandValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CommandValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    internal IEnumerable<ValidationError> GetValidationErrors()
    {
        throw new NotImplementedException();
    }
}
