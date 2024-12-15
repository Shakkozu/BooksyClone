namespace BooksyClone.Contract.Shared;

public class Result
{
    public List<string> Errors { get; set; }
    public bool Succeeded => Errors.Count > 0;

    public static Result Correct() { return new Result(); }

    public static Result ErrorResult(IList<string> errorMessages) => new() { Errors = [.. errorMessages] };
}
