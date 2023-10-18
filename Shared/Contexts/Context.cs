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
            builder.ApplyConfiguration(new UserMap());
            builder.ApplyConfiguration(new FileMap());
        }
    }
}
