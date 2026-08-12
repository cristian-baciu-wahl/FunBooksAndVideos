using FluentValidation;
using FunBooksAndVideos.API.Exceptions;
using FunBooksAndVideos.API.Filters;
using FunBooksAndVideos.API.Validators;
using FunBooksAndVideos.Application.Config;
using FunBooksAndVideos.Application.Engines;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Application.Processors;
using FunBooksAndVideos.Application.Services;
using FunBooksAndVideos.Infrastructure.Repositories;
using FunBooksAndVideos.Infrastructure.Persistence;
using FunBooksAndVideos.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using FunBooksAndVideos.Infrastructure.Publishers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register core services
builder.Services.AddScoped<IShippingSlipService, ShippingSlipService>();

// Register the in-memory shipping slip publisher as a singleton for persistence across requests
builder.Services.AddSingleton<IShippingSlipPublisher, ShippingSlipPublisher>();
builder.Services.AddScoped<IShippingSlipService, ShippingSlipService>();

builder.Services.AddScoped<ICustomerMembershipService, EfCustomerMembershipService>();
builder.Services.AddScoped<IPurchaseOrderRepository, EfPurchaseOrderRepository>();

builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

// Register all validators and filters
builder.Services.AddValidatorsFromAssemblyContaining<PurchaseOrderRequestValidator>();
builder.Services.AddScoped<ValidationFilter<PurchaseOrderRequest>>();

// Register rule engine
builder.Services.AddScoped<IBusinessRuleEngine, BusinessRuleEngine>();

// Register business rules as strategies from configuration extension
builder.Services.AddBusinessRules();

// Register processor
builder.Services.AddScoped<IPurchaseOrderProcessor, PurchaseOrderProcessor>();

// Add global exception handling and problem details middleware
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add framework health check for API -> AppDbContext -> SQL Server
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

// Configure CORS to Allow All - for production apps, we want to limit this 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add DB context and connection string
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var connectionString = builder.Configuration.GetConnectionString("FunBooksAndVideos");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FunBooksAndVideos API V1");
    });
}

app.UseExceptionHandler();

// Remove HTTPS redirection for Docker testing
// app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }