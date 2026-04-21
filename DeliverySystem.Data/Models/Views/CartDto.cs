using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Views
{
    internal class CartDto
    {
    }
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class GetCartDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Image { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class UpdateCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
