using FluentValidation;
using FunBooksAndVideos.API.Exceptions;
using FunBooksAndVideos.API.Filters;
using FunBooksAndVideos.API.Validators;
using FunBooksAndVideos.Application.DependencyInjection;
using FunBooksAndVideos.Application.PurchaseOrders.Create;
using FunBooksAndVideos.Application.PurchaseOrders.Services;
using FunBooksAndVideos.Application.PurchaseOrders.Ports;
using FunBooksAndVideos.Application.BusinessRules;
using FunBooksAndVideos.Application.BusinessRules.Ports;
using FunBooksAndVideos.Infrastructure.Fullfilment;
using FunBooksAndVideos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FunBooksAndVideos.Infrastructure.Persistence.Repositories;
using FunBooksAndVideos.Infrastructure.Persistence.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the in-memory shipping slip publisher as a singleton for persistence across requests
builder.Services.AddSingleton<IShippingSlipPublisher, InMemoryShippingSlipPublisher>();

// Register services
builder.Services.AddScoped<IShippingSlipService, ShippingSlipService>();
builder.Services.AddScoped<ICustomerMembershipService, CustomerMembershipService>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

// Register all validators and filters
builder.Services.AddValidatorsFromAssemblyContaining<PurchaseOrderRequestValidator>();
builder.Services.AddScoped<ValidationFilter<CreatePurchaseOrderRequest>>();

// Register processor, rule engine and rules
builder.Services.AddScoped<IPurchaseOrderProcessor, PurchaseOrderProcessor>();
builder.Services.AddScoped<IBusinessRuleEngine, BusinessRuleEngine>();
builder.Services.AddBusinessRules();

// Register global exception handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Register framework health check for API -> AppDbContext -> SQL Server flow
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

// Configure CORS to Allow All only in Development for testing purposes.
// For production apps, we want to limit this to specific origins, methods, and headers.
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

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var connectionString = builder.Configuration.GetConnectionString("FunBooksAndVideos");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FunBooksAndVideos API V1");
    });
}

app.UseExceptionHandler();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();