using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Views
{
    public class ProductCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public bool ImageUploaded { get; set; }
    }
    public class GetProductCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image { get; set; }
    }
}
