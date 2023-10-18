using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Communication.Mapping;
using Communication.Models;
using Communication.Services;

namespace Communication.Contexts
{
    public class Context  : IdentityDbContext<MUser, IdentityRole ,string>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        public DbSet<MUser> User { get; set; }
        public DbSet<MFile> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "Admin",
                ConcurrencyStamp = "1",
                NormalizedName = "ADMIN",
                Id = "1"
            },
            new IdentityRole
            {
                Name = "Normal",
                ConcurrencyStamp = "2",
                NormalizedName = "NORMAL",
                Id = "2"
            });
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = "42fb923f-0cb6-4df0-96b8-9aa7141a4200",
                RoleId = "1",
            });
            builder.ApplyConfiguration(new UserMap());
            builder.ApplyConfiguration(new FileMap());
        }
    }
}
