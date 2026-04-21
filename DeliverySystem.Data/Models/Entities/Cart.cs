using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
