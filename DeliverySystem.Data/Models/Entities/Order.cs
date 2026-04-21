using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Paid, Confirmed, Delivered
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
