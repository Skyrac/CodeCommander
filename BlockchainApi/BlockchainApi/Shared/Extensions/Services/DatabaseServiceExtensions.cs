using Backend.Database;
using Backend.Migrations;
using Backend.Shared;
using Backend.Shared.Interceptors;
using Backend.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlockchainApi.Shared.Extensions.Services;

public static class DatabaseServiceExtensions
{
    public static WebApplicationBuilder AddIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"]!,
                    ValidAudience = builder.Configuration["Jwt:Issuer"]!,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

        builder.Services.AddScoped<IUser, CurrentUser>();
        builder.Services.AddAuthorizationBuilder();

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, dbContextOptions) =>
        {
            var dbName = builder.Configuration.GetValue<string>("DB_NAME");
            var dbHost = builder.Configuration.GetValue<string>("DB_SERVER");
            var dbUser = builder.Configuration.GetValue<string>("DB_USER");
            var dbPw = builder.Configuration.GetValue<string>("DB_PW");
            var dbPort = builder.Configuration.GetValue<int>("DB_PORT");
            var isSsl = builder.Configuration.GetValue<string>("DB_SSL");
            var sslMode = isSsl == "true" ? "Require" : "Disable";
            var connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User Id={dbUser};Password={dbPw};SSL Mode={sslMode}";

            dbContextOptions.UseNpgsql(connectionString, config =>
            {

            });
            dbContextOptions.ConfigureWarnings(config =>
            {
                config.Ignore(RelationalEventId.AmbientTransactionWarning);
            });
            dbContextOptions.UseSnakeCaseNamingConvention();
            dbContextOptions.AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>());
        });
        builder.Services.AddScoped<ApplicationDbContextInitialiser>();
        return builder;
    }

}
