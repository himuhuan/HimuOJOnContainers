#region

using Identity.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

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