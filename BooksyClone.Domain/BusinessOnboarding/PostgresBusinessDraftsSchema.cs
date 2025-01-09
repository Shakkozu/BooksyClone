using BooksyClone.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BooksyClone.Domain.BusinessOnboarding.Model;

namespace BooksyClone.Domain.BusinessOnboarding.Storage;

internal static class PostgresBusinessDraftsSchema
{
    internal static void MapUsing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessDraft>(builder =>
        {
            builder.ToTable("business_drafts", "business_management");

            builder.MapBaseEntityProperties();

            // Map complex types as JSONB
            builder.Property(x => x.BusinessDetails)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<BusinessDetails>(v)!)
                .HasColumnName("business_details")
                .HasColumnType("jsonb");  // PostgreSQL uses JSONB for JSON

            builder.Property(x => x.UserDetails)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<UserDetails>(v)!)
                .HasColumnName("user_details")
                .HasColumnType("jsonb");

            // Map file as BYTEA
            builder.OwnsOne(x => x.BusinessProofDocument, b =>
            {
                b.Property(f => f.Data).HasColumnName("business_proof_document_data").HasColumnType("bytea");
                b.Property(f => f.FileName).HasColumnName("business_proof_document_file_name").HasMaxLength(255);
                b.Property(f => f.ContentType).HasColumnName("business_proof_document_content_type").HasMaxLength(100);
            });

            builder.OwnsOne(x => x.UserIdentityDocument, b =>
            {
                b.Property(f => f.Data).HasColumnName("user_identity_document_data").HasColumnType("bytea");
                b.Property(f => f.FileName).HasColumnName("user_identity_document_file_name").HasMaxLength(255);
                b.Property(f => f.ContentType).HasColumnName("user_identity_document_content_type").HasMaxLength(100);
            });

            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasColumnName("status");
            builder.Property(x => x.LegalConsentContent).IsRequired().HasColumnName("legal_consent_content");
            builder.Property(x => x.LegalConsent).IsRequired().HasColumnName("legal_consent");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");
        });
    }
}
