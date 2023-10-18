using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Communication.Contexts;
using Communication.Mapping;
using Communication.Models;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration["ConnectionStrings:Connection"];
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(connectionString, p => p.MigrationsAssembly("Shared"));
});


builder.Services.AddIdentity<MUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
 .AddEntityFrameworkStores<Context>()
 .AddDefaultTokenProviders();
builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.LogoutPath = "/User/Login";
    options.AccessDeniedPath = "/";
});
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}");
app.Run();
