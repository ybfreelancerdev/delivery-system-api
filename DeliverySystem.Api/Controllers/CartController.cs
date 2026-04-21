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
    [Authorize]
    public class CartController : ControllerBase
    {
        #region Property Initialization
        private Authentication.Authorization _authorization;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;
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
        public CartController(IProductCategoryService productCategoryService, 
            IProductService productService,
            ICartService cartService,
            ICartItemService cartItemService)
        {
            _authorization = new Authentication.Authorization();
            _productCategoryService = productCategoryService;
            _productService = productService;
            _cartService = cartService;
            _cartItemService = cartItemService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion

        [HttpPost("add-to-cart")]
        public async Task<OperationResult<string>> AddToCart(AddToCartDto dto)
        {
            // 1. Check product
            var product = await _productService
                .FirstOrDefaultAsync(x => x.Id == dto.ProductId && !x.IsDeleted);

            if (product == null)
                return new OperationResult<string>(false, "Product not found.");

            // 2. Get or create cart
            var cart = await _cartService
                .FirstOrDefaultAsync(x => x.UserId == LoggedInUserId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = LoggedInUserId
                };

                _cartService.Add(cart);
                await _cartService.SaveAsync();
            }

            // 3. Check if item already exists
            var cartItem = await _cartItemService
                .FirstOrDefaultAsync(x => x.CartId == cart.Id && x.ProductId == dto.ProductId);

            if (cartItem != null)
            {
                cartItem.Quantity += dto.Quantity;
                _cartItemService.UpdateAsync(cartItem, cartItem.Id);
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                _cartItemService.Add(newItem);
            }

            await _cartItemService.SaveAsync();

            return new OperationResult<string>(true, "Added to cart.");
        }

        [HttpPut("update-cart")]
        public async Task<OperationResult<string>> UpdateCart(UpdateCartDto dto)
        {
            var cart = await _cartService
                .FirstOrDefaultAsync(x => x.UserId == LoggedInUserId);

            if (cart == null)
                return new OperationResult<string>(false, "Cart not found.");

            var item = await _cartItemService
                .FirstOrDefaultAsync(x => x.CartId == cart.Id && x.ProductId == dto.ProductId);

            if (item == null)
                return new OperationResult<string>(false, "Item not found.");

            if (dto.Quantity <= 0)
            {
                _cartItemService.Delete(item);
            }
            else
            {
                item.Quantity = dto.Quantity;
                _cartItemService.UpdateAsync(item, item.Id);
            }

            await _cartItemService.SaveAsync();

            return new OperationResult<string>(true, "Cart updated.");
        }

        [HttpGet("my-cart")]
        public async Task<OperationResult<List<GetCartDto>>> GetCart()
        {
            var cart = await _cartService
                .FirstOrDefaultAsync(x => x.UserId == LoggedInUserId);

            if (cart == null)
                return new OperationResult<List<GetCartDto>>(true, "", new List<GetCartDto>());

            var items = await _cartItemService
                .FindAllAsync(x => x.CartId == cart.Id);

            var productIds = items.Select(x => x.ProductId).ToList();

            var products = await _productService
                .FindAllAsync(x => productIds.Contains(x.Id));

            var result = items.Select(item =>
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);

                return new GetCartDto
                {
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? "",
                    Image = product?.Image ?? "",
                    Price = product?.Price ?? 0,
                    Quantity = item.Quantity,
                    TotalPrice = (product?.Price ?? 0) * item.Quantity
                };
            }).ToList();

            return new OperationResult<List<GetCartDto>>(true, "", result);
        }

        [HttpDelete("remove-from-cart/{productId}")]
        public async Task<OperationResult<string>> Remove(int productId)
        {
            var cart = await _cartService
                .FirstOrDefaultAsync(x => x.UserId == LoggedInUserId);

            if (cart == null)
                return new OperationResult<string>(false, "Cart not found.");

            var item = await _cartItemService
                .FirstOrDefaultAsync(x => x.CartId == cart.Id && x.ProductId == productId);

            if (item == null)
                return new OperationResult<string>(false, "Item not found.");

            _cartItemService.Delete(item);
            await _cartItemService.SaveAsync();

            return new OperationResult<string>(true, "Item removed.");
        }
    }
}
