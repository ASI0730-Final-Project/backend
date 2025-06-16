using System.Reflection;
using FluentValidation;
using gigu_back_end.Shared.Domain;
using gigu_back_end.Shared.Infrastructure.Persistence.Configuration;
using gigu_back_end.Shared.Infraestructure.Persistence.Repositories;
using gigu_back_end.User.Application.CommandServices;
using gigu_back_end.User.Application.QueryServices;
using gigu_back_end.User.Domain;
using gigu_back_end.User.Domain.Models.Validadors;
using gigu_back_end.User.Domain.Services;
using gigu_back_end.User.Domain.Models.Exceptions;
using gigu_back_end.User.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString is null)
    throw new Exception("Database connection string is not set.");

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<GigUContext>(options =>
    {
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    });
}
else if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<GigUContext>(options =>
    {
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Error)
            .EnableDetailedErrors();
    });
}

// Shared
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Users
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();

builder.WebHost.UseUrls("http://localhost:5000");

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Learning Center app",
        Description = "API for managing users in a learning platform",
        Contact = new OpenApiContact { Name = "Naldo", Email = "naldo@example.com" },
        License = new OpenApiLicense { Name = "Example License" }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GigUContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
