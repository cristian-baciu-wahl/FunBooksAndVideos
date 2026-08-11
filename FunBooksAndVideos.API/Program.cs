using FluentValidation;
using FunBooksAndVideos.API.Filters;
using FunBooksAndVideos.API.Validators;
using FunBooksAndVideos.Application.Config;
using FunBooksAndVideos.Application.Engines;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Application.Processors;
using FunBooksAndVideos.Application.Services;
using FunBooksAndVideos.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register core services
builder.Services.AddScoped<IShippingSlipService, ShippingSlipService>();
builder.Services.AddScoped<ICustomerMembershipService, CustomerMembershipService>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
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

// Remove HTTPS redirection for Docker testing
// app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();