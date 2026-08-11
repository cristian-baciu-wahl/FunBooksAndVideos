using FunBooksAndVideos.API.Filters;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace FunBooksAndVideos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrderController(
        IPurchaseOrderProcessor orderProcessor,
        IPurchaseOrderRepository orderRepository
        ) : ControllerBase
    {

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilter<CreatePurchaseOrderRequest>))]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
        {
            var order = await orderProcessor.ProcessPurchaseOrderAsync(request);

            return Ok(new
            {
                orderId = order.Id,
                message = "Purchase order processed successfully",
                items = order.ItemLines.Count,
                totalPrice = order.TotalPrice
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrder(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid order ID" });
            }

            // refactor to be called from a service
            var order = await orderRepository.GetPurchaseOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { error = $"Purchase order with ID {id} not found" });
            }

            return Ok(new
            {
                order.Id,
                order.CustomerId,
                order.OrderDate,
                order.TotalPrice,
                ItemLines = order.ItemLines.Select(item => new
                {
                    item.Id,
                    ProductName = item.Product?.Name,
                    item.MembershipType,
                    item.Quantity,
                    item.UnitPrice,
                    TotalPrice = item.Quantity * item.UnitPrice
                })
            });
        }
    }
}