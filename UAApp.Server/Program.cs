using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;
using UAApp.Application;
using UAApp.Infrastructure.Data;
using UAApp.Persistence.DB;
using UAApp.Persistence.Seed;
using UAApp.Server.Middlewares;
using UAApp.Shared.AppSettings;
using UAApp.Shared.CurrentUser;
using UAApp.Shared.Log;

var builder = WebApplication.CreateBuilder(args);
#region Configuration file
var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
if (builder.Environment.IsDevelopment())
{
    configurationBuilder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}

var configuration = configurationBuilder.Build();
builder.Configuration.AddConfiguration(configuration);
#endregion
#region Services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IApplicationLogger, ApplicationLogger>();
builder.Host.UseSerilog((ctx, lc) => lc
       .WriteTo.Console()
       .ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddApplicationServices();
builder.Services.Configure<ApplicationSetting>(configuration);
#endregion

builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var retryPolicy = Policy
    .Handle<Exception>()
    .WaitAndRetryAsync(
        retryCount: 2,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(1, attempt)),
        onRetry: (exception, sleepDuration, retryAttempt, context) =>
        {
        });

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await retryPolicy.ExecuteAsync(async () =>
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            if (context.Database.IsSqlServer())
            {
                await context.Database.MigrateAsync();
            }
            await ApplicationDbContextSeed.SeedDataAsync(context);
        });
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<IApplicationLogger>();
        logger.Log(LogLevel.Error, new LogFormat
        {
            Message = $"Database Migration failed: {ex.Message}",
            UserID = string.Empty,
            Description = ex.InnerException?.Message ?? string.Empty,
            Method = "Application Init",
        });
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
