using Microsoft.AspNetCore.Http;

namespace BooksyClone.Domain.BusinessOnboarding.Model;

public class FileDocument
{
    public static FileDocument From(IFormFile file)
    {
        byte[] content;
        using (var memoryStream = new MemoryStream())
        {
            file.CopyToAsync(memoryStream).GetAwaiter().GetResult();
            content = memoryStream.ToArray();
        }

        return new FileDocument
        {
            ContentType = file.ContentType,
            Data = content,
            FileName = file.FileName,
        };
    }
    public byte[] Data { get; internal set; }
    public string FileName { get; internal set; }
    public string ContentType { get; internal set; }
}
