using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; } = 0;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image { get; set; } = null!;
        public decimal Price { get; set; }
        public int CreateBy { get; set; } = 0;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
