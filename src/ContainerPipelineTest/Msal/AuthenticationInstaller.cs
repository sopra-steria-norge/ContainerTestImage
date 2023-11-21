using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ContainerPipelineTest.Msal
{

    public static class AuthenticationInstaller
    {
        private const string RequiredRole = "WebApi.FullAccess";
        private const string SecuritySchema = "oauth2";

        public static void InstallAzureAdAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration, "AzureAd")                
                .EnableTokenAcquisitionToCallDownstreamApi()
                // .AddDownstreamWebApi(WebApiNames.NavData, configuration.GetSection(WebApiNames.NavData))
                .AddInMemoryTokenCaches();
        }

        public static void InstallSwaggerAuthentication(this SwaggerGenOptions swaggerOptions, IConfiguration configuration)
        {
            var options = configuration.GetSection("AzureAd");

            var scope = $"{options["ClientId"]}/Swagger.Auth";
            var tenantId = options["TenantId"];
            var instance = options["Instance"];

            var baseAuthUrl = $"{instance}{tenantId}/oauth2/v2.0/";

            swaggerOptions.AddSecurityDefinition(scope, baseAuthUrl);
            swaggerOptions.AddSecurityRequirement();
        }

        private static void AddSecurityDefinition(this SwaggerGenOptions swaggerOptions, string scope, string baseAuthUrl)
        {
            swaggerOptions.AddSecurityDefinition(SecuritySchema, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri(baseAuthUrl + "authorize"),
                        TokenUrl = new Uri(baseAuthUrl + "token"),
                        Scopes = new Dictionary<string, string> {
                        {
                            scope,
                            "Swagger access"
                        }
                    }
                    }
                }
            });
        }

        private static void AddSecurityRequirement(this SwaggerGenOptions swaggerOptions)
        {
            swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement() {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                                Id = SecuritySchema
                        },
                        Scheme = SecuritySchema,
                        Name = SecuritySchema,
                        In = ParameterLocation.Header
                },
                new List<string>()
            }
            });
        }
    }
}
