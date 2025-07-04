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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("Database connection string is not set.");

builder.Services.AddDbContext<GigUContext>(options =>
{
    options.UseMySQL(connectionString, mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors()
               .EnableSensitiveDataLogging()
               .LogTo(Console.WriteLine, LogLevel.Information);
    }
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();

builder.Services.AddScoped<IBriefcaseRepository, BriefcaseRepository>();
builder.Services.AddScoped<IBriefcaseQueryService, BriefcaseQueryService>();
builder.Services.AddScoped<IBriefcaseCommandService, BriefcaseCommandService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBriefcaseCommandValidator>();

builder.Services.AddScoped<IGigRepository, GigRepository>();
builder.Services.AddScoped<IPullRepository, PullRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();

builder.Services.AddScoped<IGigDomainService, GigDomainService>();
builder.Services.AddScoped<IPullDomainService, PullDomainService>();
builder.Services.AddScoped<IChatDomainService, ChatDomainService>();

builder.Services.AddScoped<GigCommandService>();
builder.Services.AddScoped<GigQueryService>();
builder.Services.AddScoped<IChatQueryService, ChatQueryService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateGigCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateGigCommandValidator>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "GigU Platform API",
        Description = "API for managing Users, Gigs, and Briefcases",
        Contact = new OpenApiContact
        {
            Name = "GigU Team",
            Email = "contact@gigu.app"
        },
        License = new OpenApiLicense { Name = "MIT" }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GigU API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GigUContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
