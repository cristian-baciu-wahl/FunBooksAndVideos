using FunBooksAndVideos.Application.BusinessRules;
using Microsoft.Extensions.DependencyInjection;

namespace FunBooksAndVideos.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessRules(this IServiceCollection services)
    {
        // Register all business rules
        services.AddScoped<IBusinessRule, ActivateMembershipRule>();
        services.AddScoped<IBusinessRule, GenerateShippingSlipRule>();

        // Future rules can be added here
        // services.AddScoped<IBusinessRule, ValidateCustomerStatusRule>();
        // services.AddScoped<IBusinessRule, SendOrderConfirmationRule>();
        // services.AddScoped<IBusinessRule, ApplyDiscountRule>();
        // services.AddScoped<IBusinessRule, UpdateInventoryRule>();

        return services;
    }
}