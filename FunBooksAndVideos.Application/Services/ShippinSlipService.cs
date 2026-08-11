using FunBooksAndVideos.Application.Interfaces;

namespace FunBooksAndVideos.Application.Services;

public class ShippingSlipService : IShippingSlipService
{
    public void GenerateShippingSlip(int purchaseOrderId, int customerId)
    {
        // Simulate shipping slip generation
        // E.G - Append an event to an Azure service bus for a notification service that could send SMSs, emails, WhatsApp notifications, etc.
        Console.WriteLine($"Shipping slip generated for Purchase Order {purchaseOrderId}, Customer {customerId}");
    }
}
