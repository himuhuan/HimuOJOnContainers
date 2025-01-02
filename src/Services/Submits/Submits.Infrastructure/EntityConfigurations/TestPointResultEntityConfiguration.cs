using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HimuOJ.Services.Submits.Infrastructure.EntityConfigurations;

public class TestPointResultEntityConfiguration : IEntityTypeConfiguration<TestPointResult>
{
    public void Configure(EntityTypeBuilder<TestPointResult> builder)
    {
        builder.ToTable("t_testpoint_results");

        builder.Ignore(t => t.DomainEvents);

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
               .UseHiLo("testpointresultseq");

        builder.Property(t => t.SubmissionId).IsRequired();

        builder.Property(t => t.Status)
               .HasMaxLength(20)
               .HasConversion<string>();
        
        builder.Property(t => t.TestPointId).IsRequired();
        
        builder.OwnsOne(t => t.Usage, b =>
        {
            b.Property(u => u.UsedMemoryByte)
             .HasColumnName("UsedMemoryByte");
            b.Property(u => u.UsedTimeMs)
             .HasColumnName("UsedTimeMs");
        });
        
        builder.OwnsOne(t => t.Difference, b =>
        {
            b.Property(d => d.ExpectedOutput)
             .HasColumnName("ExpectedOutput")
             .HasMaxLength(10000);
            b.Property(d => d.ActualOutput)
             .HasColumnName("ActualOutput")
             .HasMaxLength(10000);
        });
        
        builder.HasIndex(t => t.SubmissionId);
        builder.HasIndex(t => t.TestPointId);
        builder.HasIndex(t => t.Status);
    }
}