using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Plumsail.Interview.Domain.Entities;

namespace Plumsail.Interview.DatabaseContext.Configuration;

public class SubmissionEntityConfiguration : IEntityTypeConfiguration<SubmissionEntity>
{
    public void Configure(EntityTypeBuilder<SubmissionEntity> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Size)
            .IsRequired();

        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .HasConversion<string>();

        builder.Property(e => e.CreatedDate);

        builder.Property(e => e.Priority)
            .HasConversion<string>();

        builder.Property(e => e.IsPublic);
    }
}