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
        private readonly ICustomerMembershipService _customerService;

        public PurchaseOrderController(
            IPurchaseOrderProcessor orderProcessor,
            IPurchaseOrderRepository orderRepository,
            ICustomerMembershipService customerService)
        {
            _orderProcessor = orderProcessor;
            _orderRepository = orderRepository;
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request cannot be null" });
            }

            if (!_customerService.CustomerExists(request.CustomerId))
            {
                return NotFound(new { error = $"Customer with ID {request.CustomerId} not found" });
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest(new { error = "Order must contain at least one item" });
            }

            var order = new PurchaseOrder(request.Id, request.CustomerId);

            foreach (var item in request.Items)
            {
                var itemLine = new ItemLine
                {
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };

                if (item.ProductId.HasValue)
                {
                    if (!_orderRepository.ProductExists(item.ProductId.Value))
                    {
                        return BadRequest(new { error = $"Product with ID {item.ProductId.Value} not found" });
                    }

                    var product = await _orderRepository.GetProductByIdAsync(item.ProductId.Value);
                    itemLine.Product = product;
                }
                else if (!string.IsNullOrEmpty(item.MembershipType))
                {
                    if (!Enum.TryParse<MembershipType>(item.MembershipType, true, out var membershipType))
                    {
                        return BadRequest(new { error = $"Invalid membership type: {item.MembershipType}" });
                    }
                    itemLine.MembershipType = membershipType;
                }
                else
                {
                    return BadRequest(new { error = "Each item must have either a ProductId or MembershipType" });
                }

                order.ItemLines.Add(itemLine);
            }

            try
            {
                // Process the order
                _orderProcessor.ProcessPurchaseOrder(order);

                // Save the order
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
            catch (System.Exception ex)
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