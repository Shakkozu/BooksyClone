namespace BooksyClone.Contract.Dictionaries;
public class ServiceVariantDto
{
	public long Id { get; set; }

	public Guid Guid { get; set; }
	public long CategoryId { get; set; }
	public string Name { get; set; }
	public string CategoryName { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}

public class CategoryDto
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
