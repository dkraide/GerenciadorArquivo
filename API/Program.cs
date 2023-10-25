using API.CustomToken;
using Communication.Constants;
using Communication.Contexts;
using Communication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
Folders.ROOT =  
    string.IsNullOrEmpty(environment.WebRootPath) ?
    environment.ContentRootPath : environment.WebRootPath;
builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
                         "API com esquema de autorizacao. Utilize a rota /api/User/Login com seu usuario. para conseguir um token.\r\n\r\n" +
                         "Digite 'Bearer' [espaço] e então seu token no campo abaixo.\r\n\r\n" +
                         "Exemplo (informar sem as aspas): 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
var connectionString = builder.Configuration["ConnectionStrings:Connection"];
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(connectionString, p => p.MigrationsAssembly("Communication"));
});
builder.Services.AddIdentity<MUser, IdentityRole>()
 .AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();
builder.Services.AddAuthentication
(AuthOptions.DefaultScheme)
    .AddScheme<AuthOptions, AuthHandler>
    (AuthOptions.DefaultScheme,
        options => { });
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
   .AddJwtBearer(x =>
   {
       x.RequireHttpsMetadata = false;
       x.SaveToken = true;
       x.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"])),
           ValidateIssuer = false,
           ValidateAudience = false
       };
   });

builder.Services.Configure<IdentityOptions>(opcoes =>
{
    opcoes.Password.RequireDigit = false;
    opcoes.Password.RequireLowercase = false;
    opcoes.Password.RequireNonAlphanumeric = false;
    opcoes.Password.RequireUppercase = false;
    opcoes.Password.RequiredLength = 6;
    opcoes.Password.RequiredUniqueChars = 1;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DefaultModelsExpandDepth(-1);
    });
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
