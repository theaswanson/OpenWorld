using OpenWorld.Server.Authentication;
using OpenWorld.Server.Hubs;
using Serilog;
using Serilog.Events;

namespace OpenWorld.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var app = BuildApp(args);

            Log.Logger.Information("[OpenWorld Server] Starting...");

            await app.RunAsync();

            Log.Logger.Information("[OpenWorld Server] Stopped.");
        }

        private static WebApplication BuildApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureSerilog(builder);

            // Add services to the container.
            builder.Services.AddSignalR();
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IPasswordHashingService, PasswordHashingService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>("/hubs/chat");

            return app;
        }

        private static void ConfigureSerilog(WebApplicationBuilder builder)
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Is(MinimumLogLevel)
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console(restrictedToMinimumLevel: ConsoleLogLevel(builder.Environment));

            if (!builder.Environment.IsDevelopment())
            {
                loggerConfig.WriteTo.File(
                    "logs/server.log",
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    rollingInterval: RollingInterval.Day);
            }

            Log.Logger = loggerConfig.CreateLogger();

            builder.Logging.ClearProviders();
            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        }

        private static LogEventLevel MinimumLogLevel => LogEventLevel.Information;

        private static LogEventLevel ConsoleLogLevel(IHostEnvironment environment)
        {
            return environment.IsDevelopment()
                ? LogEventLevel.Information
                : LogEventLevel.Warning;
        }
    }
}
