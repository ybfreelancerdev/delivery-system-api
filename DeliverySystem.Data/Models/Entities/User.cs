using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "User";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? Otp { get; set; }
    }
}
