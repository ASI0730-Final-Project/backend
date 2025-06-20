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
using Gigs.Domain.Models.Entities;
using Gigs.Domain.Services;
using Gigs.Infrastructure.Persistence.EFC.Repositories;
using Gigs.Application.CommandService;
using Gigs.Application.QueryService;
using Gigs.Domain;
using Gigs.Domain.Models.Validators;
using gigu_back_end.Briefcases.Application.CommandServices;
using gigu_back_end.Briefcases.Application.QueryServices;
using gigu_back_end.Briefcases.Domain;
using gigu_back_end.Briefcases.Domain.Models.Validators;
using gigu_back_end.Briefcases.Domain.Services;
using gigu_back_end.Briefcases.Infraestructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración básica
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Configuración de la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString is null)
    throw new Exception("Database connection string is not set.");

builder.Services.AddDbContext<GigUContext>(options =>
{
    options.UseMySQL(connectionString, mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);
    }
});

// --- [Shared Services] ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- [Users Bounded Context] ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
builder.Services.AddScoped<IChatQueryService, ChatQueryService>(); // Chat Stuff
// --- [Briefcases Bounded Context] ---
builder.Services.AddScoped<IBriefcaseRepository, BriefcaseRepository>();
builder.Services.AddScoped<IBriefcaseQueryService, BriefcaseQueryService>();
builder.Services.AddScoped<IBriefcaseCommandService, BriefcaseCommandService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBriefcaseCommandValidator>();

// --- [Gigs Bounded Context] ---
// Repositories
builder.Services.AddScoped<IGigRepository, GigRepository>();
builder.Services.AddScoped<IPullRepository, PullRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>(); // Chat stuff

// Domain Services
builder.Services.AddScoped<IGigDomainService, GigDomainService>();
builder.Services.AddScoped<IPullDomainService, PullDomainService>(); // ✅ NUEVO: Registro de dominio Pull
builder.Services.AddScoped<IChatDomainService, ChatDomainService>(); // Chat stuff

// Application Services
builder.Services.AddScoped<GigCommandService>();
builder.Services.AddScoped<GigQueryService>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateGigCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateGigCommandValidator>();

// Configuración de Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "GigU Platform API",
        Description = "API for managing Users and Gigs",
        Contact = new OpenApiContact { Name = "Your Name", Email = "contact@example.com" },
        License = new OpenApiLicense { Name = "MIT License" }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Configurar la URL de escucha
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// Middleware y configuración HTTP
app.UseSwagger();
app.UseSwaggerUI();

// Crear base de datos si no existe (solo para desarrollo rápido)
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
