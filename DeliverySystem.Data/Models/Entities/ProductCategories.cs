using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Entities
{
    public class ProductCategories
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image { get; set; }
        public int CreatedBy { get; set; } = 0;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
