using DeliverySystem.Data.Common;
using DeliverySystem.Data.Helpers;
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
    public class ProductController : ControllerBase
    {
        #region Property Initialization
        private Authentication.Authorization _authorization;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly IFileService _fileService;
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
        public ProductController(IProductCategoryService productCategoryService, IProductService productService, IFileService fileService)
        {
            _authorization = new Authentication.Authorization();
            _productCategoryService = productCategoryService;
            _productService = productService;
            _fileService = fileService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion

        #region 'Product Category'

        [HttpPost("add-category")]
        public async Task<OperationResult<string>> Add(ProductCategoryDto dto)
        {
            var existing = await _productCategoryService.FirstOrDefaultAsync(x => x.Name.ToUpper() == dto.Name.ToUpper());
            if(existing != null) return new OperationResult<string>(false, "Product category already exists.");

            var path = await _fileService.SaveFile(
                    dto.Image,
                    "productCategories",
                    FileUploadConfig.ImageExtensions,
                    FileUploadConfig.ImageMimeTypes
                );

            var category = new ProductCategories
            {
                Name = dto.Name,
                Description = dto.Description,
                Image = path,
                CreatedBy = LoggedInUserId,
            };

            _productCategoryService.Add(category);
            await _productCategoryService.SaveAsync();

            return new OperationResult<string>(true, "Category added successfully.");
        }

        [HttpPut("update-category/{id}")]
        public async Task<OperationResult<string>> Update(int id, ProductCategoryDto dto)
        {
            var category = await _productCategoryService.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (category == null)
                return new OperationResult<string>(false, "Category not found.");

            if(dto.ImageUploaded == true)
            {
                var path = await _fileService.SaveFile(
                    dto.Image,
                    "productCategories",
                    FileUploadConfig.ImageExtensions,
                    FileUploadConfig.ImageMimeTypes
                );

                category.Image = path;
            }
            category.Name = dto.Name;
            category.Description = dto.Description;

            _productCategoryService.UpdateAsync(category, category.Id);
            await _productCategoryService.SaveAsync();

            return new OperationResult<string>(true, "Category updated successfully.");
        }

        [HttpDelete("delete-category/{id}")]
        public async Task<OperationResult<string>> Delete(int id)
        {
            var category = await _productCategoryService.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (category == null)
                return new OperationResult<string>(false, "Category not found.");

            category.IsDeleted = true;

            _productCategoryService.UpdateAsync(category, category.Id);
            await _productCategoryService.SaveAsync();

            return new OperationResult<string>(true, "Category deleted successfully.");
        }

        [HttpGet("get-category/{id}")]
        public async Task<OperationResult<GetProductCategoryDto>> GetById(int id)
        {
            var category = await _productCategoryService.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (category == null)
                return new OperationResult<GetProductCategoryDto>(false, "Category not found.");

            GetProductCategoryDto getProductCategory = new GetProductCategoryDto();
            getProductCategory.Id = category.Id;
            getProductCategory.Name = category.Name;
            getProductCategory.Description = category.Description;
            getProductCategory.Image = category.Image;

            return new OperationResult<GetProductCategoryDto>(true, "", getProductCategory);
        }

        [HttpGet("category-list")]
        [Authorize]
        public async Task<OperationResult<List<GetProductCategoryDto>>> GetAll()
        {
            var list = await _productCategoryService
                .FindAllAsync(x => !x.IsDeleted);

            var result = list.Select(x => new GetProductCategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Image = x.Image
            }).ToList();

            return new OperationResult<List<GetProductCategoryDto>>(true, "", result);
        }

        #endregion

        #region 'Product'

        [HttpPost("add-product")]
        public async Task<OperationResult<string>> Add(ProductDto dto)
        {
            var exists = await _productService
                .FirstOrDefaultAsync(x => x.CategoryId == dto.CategoryId && x.Name.ToUpper() == dto.Name.ToUpper() && !x.IsDeleted);

            if (exists != null)
                return new OperationResult<string>(false, "Product already exists.");

            var path = await _fileService.SaveFile(
                dto.Image,
                "products",
                FileUploadConfig.ImageExtensions,
                FileUploadConfig.ImageMimeTypes
            );

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Image = path,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                CreateBy = LoggedInUserId
            };

            _productService.Add(product);
            await _productService.SaveAsync();

            return new OperationResult<string>(true, "Product added successfully.");
        }

        [HttpPut("update-product/{id}")]
        public async Task<OperationResult<string>> Update(int id, ProductDto dto)
        {
            var product = await _productService
                .FirstOrDefaultAsync(x => x.Id == id && x.CategoryId == dto.CategoryId && !x.IsDeleted);

            if (product == null)
                return new OperationResult<string>(false, "Product not found.");

            if (dto.ImageUploaded == true)
            {
                var path = await _fileService.SaveFile(
                    dto.Image,
                    "products",
                    FileUploadConfig.ImageExtensions,
                    FileUploadConfig.ImageMimeTypes
                );

                product.Image = path;
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;

            _productService.UpdateAsync(product, product.Id);
            await _productService.SaveAsync();

            return new OperationResult<string>(true, "Product updated successfully.");
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<OperationResult<string>> DeleteProduct(int id)
        {
            var product = await _productService
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (product == null)
                return new OperationResult<string>(false, "Product not found.");

            product.IsDeleted = true;

            _productService.UpdateAsync(product, product.Id);
            await _productService.SaveAsync();

            return new OperationResult<string>(true, "Product deleted successfully.");
        }

        [HttpGet("get-product/{id}")]
        public async Task<OperationResult<GetProductDto>> GetProductById(int id)
        {
            var product = await _productService
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (product == null)
                return new OperationResult<GetProductDto>(false, "Product not found.");

            var result = new GetProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                Price = product.Price,
                CategoryId = product.CategoryId
            };

            return new OperationResult<GetProductDto>(true, "", result);
        }

        [HttpGet("product-list")]
        public async Task<OperationResult<List<GetProductDto>>> GetAllProducts()
        {
            var list = await _productService
                .FindAllAsync(x => !x.IsDeleted);

            var result = list.Select(x => new GetProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Image = x.Image,
                Price = x.Price,
                CategoryId = x.CategoryId
            }).ToList();

            return new OperationResult<List<GetProductDto>>(true, "", result);
        }

        [HttpGet("products-by-category/{categoryId}")]
        public async Task<OperationResult<List<GetProductDto>>> GetProductsByCategory(int categoryId)
        {
            var list = await _productService
                .FindAllAsync(x => x.CategoryId == categoryId && !x.IsDeleted);

            var result = list.Select(x => new GetProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Image = x.Image,
                Price = x.Price,
                CategoryId = x.CategoryId
            }).ToList();

            return new OperationResult<List<GetProductDto>>(true, "", result);
        }

        [HttpGet("products")]
        public async Task<OperationResult<List<GetProductDto>>> GetProducts(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int page = 1,
        int pageSize = 10)
        {
            var query = _productService.Query().Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.Name.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId);

            if (minPrice.HasValue)
                query = query.Where(x => x.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(x => x.Price <= maxPrice);

            var result = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Image = x.Image,
                    Price = x.Price,
                    CategoryId = x.CategoryId
                }).ToList();

            return new OperationResult<List<GetProductDto>>(true, "", result);
        }

        #endregion
    }
}
