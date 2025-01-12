namespace BooksyClone.Domain.Availability;

public class Result
{
	public List<Error> Errors { get; set; } = [];
    public bool Succeeded => Errors.Count == 0;

    public static Result Correct() { return new Result(); }

    public static Result ErrorResult(IList<Error> errorMessages) => new() { Errors = [..errorMessages] };
    public static Result ErrorResult(Error errorMessage) => new() { Errors = [errorMessage] };
}

public record Error(string Message, string Code);
