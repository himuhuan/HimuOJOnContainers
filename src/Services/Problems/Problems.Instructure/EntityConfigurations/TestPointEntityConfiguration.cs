#region

using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace HimuOJ.Services.Problems.Infrastructure.EntityConfigurations;

public class TestPointEntityConfiguration : IEntityTypeConfiguration<TestPoint>
{
    public void Configure(EntityTypeBuilder<TestPoint> builder)
    {
        builder.ToTable("t_testpoints");
        builder.Ignore(tp => tp.DomainEvents);

        builder.Property(tp => tp.Id).UseHiLo("testpointseq");

        builder.Property(tp => tp.ProblemId)
            .IsRequired();

        builder.Property(tp => tp.Remarks)
            .HasMaxLength(2000);
    }
}