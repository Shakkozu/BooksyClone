using BooksyClone.Domain.Availability;

namespace BooksyClone.Domain.Shared;

public static class ErrorExtensions
{
    public static Result ToErrorResult(this Error error)
    {
        return Result.ErrorResult(error);
    }
}