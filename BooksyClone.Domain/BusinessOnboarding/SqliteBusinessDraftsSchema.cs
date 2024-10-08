﻿using BooksyClone.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BooksyClone.Domain.BusinessOnboarding.Model;

namespace BooksyClone.Domain.BusinessOnboarding.Storage;

internal static class SqliteBusinessDraftsSchema
{
    internal static void MapUsing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessDraft>(builder =>
        {
            builder.MapBaseEntityProperties();

            // Map complex types as JSON
            builder.Property(x => x.BusinessDetails)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<BusinessDetails>(v))
                .HasColumnType("TEXT");  // SQLite uses TEXT for JSON

            builder.Property(x => x.UserDetails)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<UserDetails>(v))
                .HasColumnType("TEXT");

            // Map file as BLOB
            builder.OwnsOne(x => x.BusinessProofDocument, b =>
            {
                b.Property(f => f.Data).HasColumnType("BLOB");
                b.Property(f => f.FileName).HasMaxLength(255);
                b.Property(f => f.ContentType).HasMaxLength(100);
            });

            builder.OwnsOne(x => x.UserIdentityDocument, b =>
            {
                b.Property(f => f.Data).HasColumnType("BLOB");
                b.Property(f => f.FileName).HasMaxLength(255);
                b.Property(f => f.ContentType).HasMaxLength(100);
            });

            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LegalConsentContent).IsRequired();
            builder.Property(x => x.LegalConsent).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        });
    }
}
