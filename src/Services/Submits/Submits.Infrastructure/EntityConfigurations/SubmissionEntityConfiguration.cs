#region

using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace HimuOJ.Services.Submits.Infrastructure.EntityConfigurations;

public class SubmissionEntityConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("t_submissions");

        builder.Ignore(s => s.DomainEvents);

        builder.Property(s => s.Id)
            .UseHiLo("submissionseq");

        // The corresponding problem may be deleted
        builder.Property(s => s.ProblemId)
            .IsRequired(false);

        builder.Property(s => s.SubmitterId)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(s => s.SourceCode)
            .HasMaxLength(10000)
            .IsRequired();

        builder.Property(s => s.CompilerName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Status)
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.StatusMessage)
            .HasMaxLength(10000);

        builder.OwnsOne(s => s.Usage, b =>
        {
            b.Property(u => u.UsedMemoryByte)
                .HasColumnName("UsedMemoryByte");
            b.Property(u => u.UsedTimeMs)
                .HasColumnName("UsedTimeMs");
        });

        builder.HasIndex(s => s.SubmitTime);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.SubmitterId);
        builder.HasIndex(s => s.CompilerName);
        builder.HasIndex(s => s.ProblemId);


        builder.HasMany(s => s.TestPointResults)
            .WithOne()
            .HasForeignKey(t => t.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}