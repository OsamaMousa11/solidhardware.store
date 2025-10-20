using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using solidhardware.storeCore.Domain.IdentityEntites;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.AuthenticationDTO;
using solidhardware.storeCore.Service;
using solidhardware.storeCore.ServiceContract;
using solidhardware.storeinfrastraction.Data;
using System;
using System.Text;

namespace solidhardware.storeApi.StartUp
{
    public static class ConfigureServiceExtension
    {
        public static IServiceCollection ServiceConfiguration(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("connstr") ??
                throw new InvalidOperationException("Connection string 'connstr' not found."));
            });

            Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 5;
            })
              .AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders()
              .AddUserStore<UserStore<ApplicationUser, ApplicationRole, AppDbContext, Guid>>()
              .AddRoleStore<RoleStore<ApplicationRole, AppDbContext, Guid>>();
            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(o =>
               {
                   o.RequireHttpsMetadata = false;
                   o.SaveToken = false;
                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidIssuer = Configuration["JWT:Issuer"],
                       ValidAudience = Configuration["JWT:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
                       ClockSkew = TimeSpan.Zero
                   };
               });
            Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(1);
            });
            Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });


            Services.Configure<JwtDTO>(Configuration.GetSection("JWT"));
            Services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            Services.AddTransient<IMailingService, MailingService>();
            Services.AddScoped<IAuthenticationService, AuthenticationService>(); 






            //  Services.AddAutoMapper(typeof(EmployeeConfig).Assembly);
            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Employee Management ",
                    Version = "v1",
                    Description = "Employee Management API"
                });
            });


            return Services;
        }
    }
}
