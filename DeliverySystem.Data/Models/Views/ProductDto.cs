using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Views
{
    public class ProductDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile Image { get; set; } = null!;
        public decimal Price { get; set; }
        public bool ImageUploaded { get; set; }
    }
    public class GetProductDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
