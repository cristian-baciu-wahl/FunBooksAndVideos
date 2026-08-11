using FunBooksAndVideos.API.Filters;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Mappers;
using FunBooksAndVideos.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace FunBooksAndVideos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrderController(
        IPurchaseOrderProcessor orderProcessor,
        IPurchaseOrderService orderService
        ) : ControllerBase
    {
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilter<PurchaseOrderRequest>))]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] PurchaseOrderRequest request)
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
            if (id <= 0) return BadRequest(new { error = "Invalid order ID" });

            var order = await orderService.GetPurchaseOrderByIdAsync(id);
            if (order == null) return NotFound(new { error = $"Purchase order with ID {id} not found" });

            return Ok(order.ToResponse());
        }
    }
}