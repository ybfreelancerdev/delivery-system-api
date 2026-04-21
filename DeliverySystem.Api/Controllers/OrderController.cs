using DeliverySystem.Data.Common;
using DeliverySystem.Data.Models.Entities;
using DeliverySystem.Data.Models.Views;
using DeliverySystem.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DeliverySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        #region Property Initialization
        private Authentication.Authorization _authorization;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IUserService _userService;
        private readonly IUserAddressService _userAddressService;
        private int LoggedInUserId
        {
            get
            {
                ClaimsPrincipal userClaims = this.User as ClaimsPrincipal;
                return _authorization.GetUserId(userClaims);
            }
        }
        #endregion

        #region 'Constructor'
        public OrderController(IProductCategoryService productCategoryService,
            IProductService productService,
            ICartService cartService,
            ICartItemService cartItemService,
            IOrderService orderService,
            IOrderItemService orderItemService,
            IUserService userService,
            IUserAddressService userAddressService)
        {
            _authorization = new Authentication.Authorization();
            _productCategoryService = productCategoryService;
            _productService = productService;
            _cartService = cartService;
            _cartItemService = cartItemService;
            _orderService = orderService;
            _orderItemService = orderItemService;
            _userService = userService;
            _userAddressService = userAddressService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion

        [HttpPost("checkout")]
        public async Task<OperationResult<string>> Checkout(int addressId)
        {
            var cart = await _cartService
                .FirstOrDefaultAsync(x => x.UserId == LoggedInUserId);

            if (cart == null)
                return new OperationResult<string>(false, "Cart not found.");

            var cartItems = await _cartItemService
                .FindAllAsync(x => x.CartId == cart.Id);

            if (!cartItems.Any())
                return new OperationResult<string>(false, "Cart is empty.");

            var productIds = cartItems.Select(x => x.ProductId).ToList();

            var products = await _productService
                .FindAllAsync(x => productIds.Contains(x.Id));

            decimal totalAmount = 0;
            var order = new Order
            {
                UserId = LoggedInUserId,
                Status = "Pending",
                AddressId = addressId
            };

            var orderData = _orderService.Add(order);
            await _orderService.SaveAsync();

            foreach (var item in cartItems)
            {
                var product = products.First(p => p.Id == item.ProductId);

                totalAmount += product.Price * item.Quantity;

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                };

                _orderItemService.Add(orderItem);
            }
            order.Id = orderData.Id;
            order.TotalAmount = totalAmount;

            _orderService.UpdateAsync(order, order.Id);

            // Clear cart
            foreach (var item in cartItems)
                _cartItemService.Delete(item);

            await _cartItemService.SaveAsync();
            await _orderService.SaveAsync();
            await _orderItemService.SaveAsync();

            return new OperationResult<string>(true, "");
        }

        [HttpPost("make-payment/{orderId}")]
        public async Task<OperationResult<string>> MakePayment(int orderId)
        {
            var order = await _orderService
                .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == LoggedInUserId);

            if (order == null)
                return new OperationResult<string>(false, "Order not found.");

            if (order.Status != "Pending")
                return new OperationResult<string>(false, "Invalid order state.");

            // Simulate payment success
            order.Status = "Paid";

            _orderService.UpdateAsync(order, order.Id);
            await _orderService.SaveAsync();

            return new OperationResult<string>(true, "Payment successful.");
        }

        [HttpPut("cancel-order/{orderId}")]
        public async Task<OperationResult<string>> CancelOrder(int orderId)
        {
            var order = await _orderService
                .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == LoggedInUserId);

            if (order == null)
                return new OperationResult<string>(false, "Order not found.");

            if (order.Status == "Delivered" || order.Status == "Cancelled")
                return new OperationResult<string>(false, "Order cannot be cancelled.");

            order.Status = "Cancelled";

            _orderService.UpdateAsync(order, order.Id);
            await _orderService.SaveAsync();

            return new OperationResult<string>(true, "Order cancelled successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-order-status/{orderId}")]
        public async Task<OperationResult<string>> UpdateStatus(int orderId, OrderStatus status)
        {
            var order = await _orderService
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
                return new OperationResult<string>(false, "Order not found.");

            order.Status = status.ToString();

            _orderService.UpdateAsync(order, order.Id);
            await _orderService.SaveAsync();

            return new OperationResult<string>(true, "Order status updated.");
        }

        [HttpGet("my-orders")]
        public async Task<OperationResult<List<GetOrderDto>>> MyOrders()
        {
            var orders = await _orderService
                .FindAllAsync(x => x.UserId == LoggedInUserId);

            var orderIds = orders.Select(x => x.Id).ToList();

            var orderItems = await _orderItemService
                .FindAllAsync(x => orderIds.Contains(x.OrderId));

            var result = orders.Select(order => new GetOrderDto
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedOn = order.CreatedOn,
                Items = orderItems
                    .Where(x => x.OrderId == order.Id)
                    .Select(i => new GetOrderItemDto
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
            }).ToList();

            return new OperationResult<List<GetOrderDto>>(true, "", result);
        }

        [HttpGet("order-details/{orderId}")]
        public async Task<OperationResult<GetOrderDetailsDto>> GetOrderDetails(int orderId)
        {
            var order = await _orderService
                .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == LoggedInUserId);

            if (order == null)
                return new OperationResult<GetOrderDetailsDto>(false, "Order not found.");

            var orderItems = await _orderItemService
                .FindAllAsync(x => x.OrderId == order.Id);

            var productIds = orderItems.Select(x => x.ProductId).ToList();

            var products = await _productService
                .FindAllAsync(x => productIds.Contains(x.Id));

            var items = orderItems.Select(item =>
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);

                return new GetOrderItemDetailsDto
                {
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? "",
                    Image = product?.Image ?? "",
                    Price = item.Price,
                    Quantity = item.Quantity,
                    TotalPrice = item.Price * item.Quantity
                };
            }).ToList();

            var result = new GetOrderDetailsDto
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedOn = order.CreatedOn,
                Items = items
            };

            return new OperationResult<GetOrderDetailsDto>(true, "", result);
        }

        [HttpGet("order-summary")]
        public async Task<OperationResult<object>> OrderSummary()
        {
            var cart = await _cartService
                .FirstOrDefaultAsync(x => x.UserId == LoggedInUserId);

            if (cart == null)
                return new OperationResult<object>(false, "Cart not found.");

            var items = await _cartItemService
                .FindAllAsync(x => x.CartId == cart.Id);

            var productIds = items.Select(x => x.ProductId).ToList();

            var products = await _productService
                .FindAllAsync(x => productIds.Contains(x.Id));

            var result = items.Select(i =>
            {
                var p = products.First(x => x.Id == i.ProductId);
                return new
                {
                    p.Name,
                    p.Price,
                    i.Quantity,
                    Total = p.Price * i.Quantity
                };
            }).ToList();

            var total = result.Sum(x => x.Total);

            return new OperationResult<object>(true, "", new
            {
                Items = result,
                GrandTotal = total
            });
        }

        [HttpGet("all-orders")]
        [Authorize(Roles = "Admin")]
        public async Task<OperationResult<List<AdminOrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.FindAllAsync(x => true);

            var userIds = orders.Select(x => x.UserId).Distinct().ToList();
            var addressIds = orders.Select(x => x.AddressId).Distinct().ToList();
            var orderIds = orders.Select(x => x.Id).ToList();

            var users = await _userService.FindAllAsync(x => userIds.Contains(x.Id));
            var addresses = await _userAddressService.FindAllAsync(x => addressIds.Contains(x.Id));
            var orderItems = await _orderItemService.FindAllAsync(x => orderIds.Contains(x.OrderId));

            var productIds = orderItems.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productService.FindAllAsync(x => productIds.Contains(x.Id));

            var result = orders.Select(order =>
            {
                var user = users.FirstOrDefault(u => u.Id == order.UserId);
                var address = addresses.FirstOrDefault(a => a.Id == order.AddressId);

                var items = orderItems
                    .Where(x => x.OrderId == order.Id)
                    .Select(i =>
                    {
                        var product = products.FirstOrDefault(p => p.Id == i.ProductId);

                        return new AdminOrderItemDto
                        {
                            ProductName = product?.Name ?? "",
                            Quantity = i.Quantity,
                            Price = i.Price
                        };
                    }).ToList();

                return new AdminOrderDto
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    UserName = user?.Name ?? "",
                    AddressId = order.AddressId,
                    FullAddress = address != null
                        ? $"{address.Address}, {address.City}, {address.Pincode}"
                        : "",
                    Status = order.Status.ToString(),
                    CreatedOn = order.CreatedOn,
                    Items = items
                };
            }).ToList();

            return new OperationResult<List<AdminOrderDto>>(true, "", result);
        }
    }
}
