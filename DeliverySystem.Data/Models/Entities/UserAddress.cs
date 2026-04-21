using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Entities
{
    public class UserAddress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
