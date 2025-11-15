using Microsoft.EntityFrameworkCore;

using Plumsail.Interview.DatabaseContext.Configuration;
using Plumsail.Interview.Domain.Entities;

namespace Plumsail.Interview.DatabaseContext;

public class PlumsailDbContext(DbContextOptions<PlumsailDbContext> options) : DbContext(options)
{
    public DbSet<SubmissionEntity> Submissions { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new SubmissionEntityConfiguration());
    }
}