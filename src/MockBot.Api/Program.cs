using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MockBot.Api.Configuration;
using MockBot.Api.Interfaces;
using MockBot.Api.Services;
using MockBot.Api.Services.Dmr;
using MockBot.Api.Services.Dmr.Extensions;
using RequestProcessor.Services.Encoder;

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

            // Add services to the container.
            var dmrSettings = configuration.GetSection("DmrServiceSettings").Get<DmrServiceSettings>();
            services.AddDmrService(dmrSettings);

            var botSettings = configuration.GetSection("BotSettings").Get<BotSettings>();
            services.TryAddSingleton(botSettings);

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

            _ = app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }
    }
}