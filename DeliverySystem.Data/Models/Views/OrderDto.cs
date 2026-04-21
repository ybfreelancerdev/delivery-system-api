using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Models.Views
{
    public class OrderDto
    {
    }
    public class GetOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class GetOrderDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public List<GetOrderItemDto> Items { get; set; } = new();
    }
    public class GetOrderDetailsDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public List<GetOrderItemDetailsDto> Items { get; set; } = new();
    }
    public class GetOrderItemDetailsDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Image { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public enum OrderStatus
    {
        Pending,
        Paid,
        Confirmed,
        OutForDelivery,
        Delivered,
        Cancelled
    }
    public class AdminOrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int AddressId { get; set; }
        public string FullAddress { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public List<AdminOrderItemDto> Items { get; set; } = new();
    }
    public class AdminOrderItemDto
    {
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
