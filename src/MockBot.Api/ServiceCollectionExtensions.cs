using AspNetCore.Authentication.ApiKey;
using Buerokratt.Common.CentOps;
using Buerokratt.Common.Dmr;
using Buerokratt.Common.Dmr.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace MockBot.Api
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDmrCommunications(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // Add services to the container.
            var configurationSectionName = "DmrServiceSettings";
            var dmrSettings = configuration.GetSection(configurationSectionName).Get<DmrServiceSettings>();
            var centOpsSettings = configuration.GetSection(configurationSectionName).Get<CentOpsServiceSettings>();
            services.AddDmrService(dmrSettings, centOpsSettings);
        }

        public static void AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var configuredApiKey = configuration.GetConnectionString("ApiKey");
            if (string.IsNullOrWhiteSpace(configuredApiKey))
            {
                throw new ArgumentException("ConnectionStrings::ApiKey not specified.");
            }

            _ = services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                        .AddApiKeyInHeader(options =>
                        {
                            options.KeyName = "X-Api-Key";
                            options.Realm = "Mockbot";
                            options.IgnoreAuthenticationIfAllowAnonymous = true;
                            options.Events = new ApiKeyEvents
                            {
                                OnValidateKey = context =>
                                {
                                    if (string.IsNullOrEmpty(context.ApiKey) ||
                                        context.ApiKey != configuredApiKey)
                                    {
                                        context.ValidationFailed();

                                    }
                                    else
                                    {
                                        context.ValidationSucceeded();
                                    }

                                    return Task.CompletedTask;
                                }
                            };
                        });

            _ = services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }
    }
}