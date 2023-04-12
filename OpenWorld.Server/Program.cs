using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenWorld.Server.Authentication;
using OpenWorld.Server.Hubs;
using Serilog;
using Serilog.Events;
using System.Text;

namespace OpenWorld.Server
{
    public class Program
    {
        // TODO: store signing key in configuration
        private const string TokenSigningKey = "KEY2KEY2KEY2KEY2";
        
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

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Administrator");
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // TODO: store issuer and audience in configuration
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "OpenWorld Authority",
                        ValidAudience = "OpenWorld Client",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSigningKey))
                    };

                    // We have to hook the OnMessageReceived event in order to
                    // allow the JWT authentication handler to read the access
                    // token from the query string when a WebSocket or 
                    // Server-Sent Events request comes in.

                    // Sending the access token in the query string is required due to
                    // a limitation in Browser APIs. We restrict it to only calls to the
                    // SignalR hub in this code.
                    // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
                    // for more information about security considerations when using
                    // the query string to transmit the access token.
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
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
