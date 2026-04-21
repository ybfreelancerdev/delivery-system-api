using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Views
{
    public class UserAddressDto
    {
    }
    public class AddressDto
    {
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public bool IsDefault { get; set; } = false;
    }
    public class GetAddressDto
    {
        public int Id { get; set; }
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
    }
}
