using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Communication.Models;

namespace Communication.Mapping
{
    public class UserMap : IEntityTypeConfiguration<MUser>
    {
        public void Configure(EntityTypeBuilder<MUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Files);
            builder.Ignore(x => x.EmailConfirmed);
            builder.Ignore(x => x.PhoneNumberConfirmed);
            builder.Ignore(x => x.PhoneNumber);
            builder.Ignore(x => x.AccessFailedCount);
            builder.Ignore(x => x.NormalizedEmail);
            builder.Ignore(x => x.Email);
            builder.ToTable("User");
        }


    }
}