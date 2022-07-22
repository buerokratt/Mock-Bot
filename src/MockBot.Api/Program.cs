using Buerokratt.Common.Encoder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MockBot.Api.Configuration;
using MockBot.Api.Interfaces;
using MockBot.Api.Services;
using System.Diagnostics.CodeAnalysis;

namespace MockBot.Api
{
    [ExcludeFromCodeCoverage] // This is not solution code, no need for unit tests
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            var services = builder.Services;

            _ = configuration.AddEnvironmentVariables();

            var botSettings = configuration.GetSection("BotSettings").Get<BotSettings>();
            services.TryAddSingleton(botSettings);

            services.AddDmrCommunications(configuration);
            services.AddApiAuthentication(configuration);

            _ = services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = services.AddEndpointsApiExplorer();
            _ = services.AddSwaggerGen();
            _ = services.AddSingleton<IChatService, ChatService>();
            _ = services.AddSingleton<IEncodingService, EncodingService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI(options =>
                {
                    options.EnableTryItOutByDefault();
                });
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }
    }
}