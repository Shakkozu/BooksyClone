using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksyClone.Domain.Storage;
public class BaseEntity
{
    public long? Id { get; protected set; }
    public int Version { get; set; } = 0;
    public Guid Guid { get; set; }

    public override int GetHashCode()
    {
        return GetType().GetHashCode();
    }

}


public static class EfCoreExtensions
{
	public static void MapBaseEntityProperties<T>(this EntityTypeBuilder<T> builder) where T : BaseEntity
	{
		builder.Property(x => x.Id).HasColumnName("id");
		builder.Property(e => e.Guid).HasColumnName("guid");
		builder.Property(e => e.Version)
			.HasColumnName("version")
			.IsConcurrencyToken();
	}
}
