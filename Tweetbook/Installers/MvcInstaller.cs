using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Tweetbook.Filters;
using Tweetbook.Options;
using Tweetbook.Services;

namespace Tweetbook.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            var jwtSettings = new JwtSettingsOptions();
            configuration.GetSection(nameof(JwtSettingsOptions)).Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddControllersWithViews(config => config.Filters.Add<ValidationFilter>())
                .AddFluentValidation(config => 
                    config.RegisterValidatorsFromAssemblyContaining<Program>());

            services.Configure<ApiBehaviorOptions>(config =>
            {
                config.SuppressModelStateInvalidFilter = true;
            });

            var tokenParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuerSigningKey = true
            };

            services.AddSingleton(tokenParams);

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.TokenValidationParameters = tokenParams;
            });

            services.AddScoped<IUriService, UriService>(provider => 
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                return new UriService($"{request.Scheme}://{request.Host.ToUriComponent()}");
            });
        }
    }
}
