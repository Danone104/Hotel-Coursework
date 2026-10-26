using Hotel.DataAccess.Security;
using Hotel.Domain.Abstractions.Security;
using Hotel.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


namespace Hotel.API.Extensions
{
        public static class AuthenticationExtensions
        {
            public static void AddJwtAuthentication(
                this IServiceCollection services,
                IConfiguration configuration,
                string secretKey)
            {

                var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
                jwtOptions.SecretKey=secretKey;
                services.AddSingleton(jwtOptions);

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,

                            ValidateAudience = false,

                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtOptions!.SecretKey)),
                            RoleClaimType = ClaimTypes.Role

                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var token = context.Request.Cookies["access_token"];
                                context.Token=token;
                                return Task.CompletedTask;
                            },

                        };
                    });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Client", policy =>
                        policy.RequireRole(nameof(Roles.Client)));

                    options.AddPolicy("Manager", policy =>
                        policy.RequireRole(nameof(Roles.Manager)));

                    options.AddPolicy("Admin", policy =>
                        policy.RequireRole(nameof(Roles.Admin)));
                    });

            }
        }
    }
