using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Identity.Server.Models;

namespace Identity.Server.Data.EntityConfiguration
{
    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.Avatar).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Background).HasMaxLength(256).IsRequired();
        }
    }
}
