using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace FunBooksAndVideos.Application.Config
{
    public static class RuleConfiguration
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
}