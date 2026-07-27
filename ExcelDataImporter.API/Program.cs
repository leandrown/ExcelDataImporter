using ExcelDataImporter.Application.Interfaces;
using ExcelDataImporter.Application.Services;
using ExcelDataImporter.Infrastructure.Data;
using ExcelDataImporter.Infrastructure.Repositories;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext();
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in the configuration. Check your appsettings.json or appsettings.Development.json.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IImportRepository, ImportRepository>();
builder.Services.AddScoped<IImportService, ImportService>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// SqlServer check runs a direct SELECT 1; DbContext check goes through EF Core's
// CanConnectAsync. Keeping both catches issues that could affect only one of the two paths.
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString,
        name: "SQL Server",
        tags: ["db", "sql", "ready"])
    .AddDbContextCheck<AppDbContext>(
         name: "ef-core-dbcontext",
         tags: ["db", "ready"]);

var app = builder.Build();

// Serilog request logging - logs each HTTP request with method, path, status code, and execution time.
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Excel Data Importer API");
        options.WithTheme(ScalarTheme.Saturn);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// No checks run here — only confirms the process is up, for use as a liveness probe
app.MapHealthChecks("/api/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();
