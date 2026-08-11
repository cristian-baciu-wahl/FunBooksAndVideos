using FunBooksAndVideos.API.Filters;
using FunBooksAndVideos.API.Models;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;
using FunBooksAndVideos.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FunBooksAndVideos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderProcessor _orderProcessor;
        private readonly IPurchaseOrderRepository _orderRepository;

        public PurchaseOrderController(
            IPurchaseOrderProcessor orderProcessor,
            IPurchaseOrderRepository orderRepository
        )
        {
            _orderProcessor = orderProcessor;
            _orderRepository = orderRepository;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilter<CreatePurchaseOrderRequest>))]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
        {
            var order = new PurchaseOrder(request.Id, request.CustomerId);

            foreach (var item in request.Items)
            {
                var itemLine = new ItemLine
                {
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };

                var product = await _orderRepository.GetProductByIdAsync(item.ProductId.GetValueOrDefault());
                itemLine.Product = product;

                Enum.TryParse<MembershipType>(item.MembershipType, true, out var membershipType);
                itemLine.MembershipType = membershipType;

                order.ItemLines.Add(itemLine);
            }

            try
            {
                _orderProcessor.ProcessPurchaseOrder(order);

                await _orderRepository.SavePurchaseOrderAsync(order);

                return Ok(new
                {
                    orderId = order.Id,
                    message = "Purchase order processed successfully",
                    items = order.ItemLines.Count,
                    totalPrice = order.TotalPrice
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred processing your order", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrder(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid order ID" });
            }

            var order = await _orderRepository.GetPurchaseOrderByIdAsync(id);
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