using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

using Plumsail.Interview.Domain.Entities;

namespace Plumsail.Interview.DatabaseContext.Configuration;

public class SubmissionEntityConfiguration : IEntityTypeConfiguration<SubmissionEntity>
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public void Configure(EntityTypeBuilder<SubmissionEntity> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.FileData)
            .HasConversion(
                new ValueConverter<FileData?, string?>(
                    v => v.HasValue ? JsonSerializer.Serialize(v.Value, JsonOptions) : null,
                    v => !string.IsNullOrEmpty(v)
                        ? JsonSerializer.Deserialize<FileData>(v, JsonOptions)
                        : null))
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.Payload)
            .HasConversion(
                new ValueConverter<JsonElement, string>(
                    v => v.ValueKind != JsonValueKind.Undefined ? v.GetRawText() : "{}",
                    v => !string.IsNullOrEmpty(v) && v != "{}" 
                        ? JsonSerializer.Deserialize<JsonElement>(v, JsonOptions) 
                        : default(JsonElement)))
            .HasColumnType("nvarchar(max)");
    }
}