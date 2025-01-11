#region

using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace HimuOJ.Services.Problems.Infrastructure.EntityConfigurations;

class ProblemEntityConfiguration : IEntityTypeConfiguration<Problem>
{
    public void Configure(EntityTypeBuilder<Problem> builder)
    {
        builder.ToTable("t_problems");

        builder.Ignore(p => p.DomainEvents);

        builder.Property(p => p.Id).UseHiLo("problemseq");

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.DistributorId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.DistributorId);

        builder.HasMany(p => p.TestPoints)
            .WithOne()
            .HasForeignKey(t => t.ProblemId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.OwnsOne(p => p.DefaultResourceLimit);
        builder.OwnsOne(p => p.GuestAccessLimit);
    }
}