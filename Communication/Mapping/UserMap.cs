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
            builder.ToTable("User").HasData( new MUser
            {
                Id = "42fb923f-0cb6-4df0-96b8-9aa7141a4200",
                Name = "Daniel Kriade",
                UserName = "daniel123",
                NormalizedUserName = "DANIEL123",
                PasswordHash = "AQAAAAIAAYagAAAAECg2WVF3Xpo6LRu+LhBng388Cxa5FTGW3oga6qhBdFgIasEu9drf7IEQrKTE6SjsBw==",
                SecurityStamp = "RXBL52C6EPPSYEXRGMGY476RLRDQTOTF",
                ConcurrencyStamp = "7831dd0c-de10-4368-8f5e-dbf0442691f9",
                TwoFactorEnabled = false,
                LockoutEnabled = true
            });
        }


    }
}