using Microsoft.EntityFrameworkCore;
using Polly;
using UAApp.Application;
using UAApp.Domain.Common;
using UAApp.Infrastructure.Data;
using UAApp.Persistence.DB;
using UAApp.Persistence.Seed;
using UAApp.Shared.Log;
using UAApp.Shared.Services;

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
builder.Services.AddScoped<IApplicationLogger, ApplicationLogger>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddApplicationServices();
builder.Services.Configure<ApplicationSetting>(configuration);
#endregion

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
        //var logger = services.GetRequiredService<ILogger<Program>>();
        //logger.LogCritical(LoggingEvents.INIT_DATABASE, ex, LoggingEvents.INIT_DATABASE.Name);

        //throw new Exception(LoggingEvents.INIT_DATABASE.Name, ex);
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

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
