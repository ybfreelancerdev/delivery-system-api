using DeliverySystem.Data.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DeliverySystem.Data.Context
{
    public class DeliverySystemContext : DbContext
    {
        public static string? ConnectionString { get; set; }
        public static string? ValidIssuer { get; set; }
        public static string? SecretKey { get; set; }
        public static string? ValidAudience { get; set; }
        public static string? TokenExpireMinute { get; set; }

        public DeliverySystemContext(DbContextOptions<DeliverySystemContext> options)
        : base(options) {
            Database.SetCommandTimeout(150000);
        }

        public DeliverySystemContext()
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
        public DbSet<ProductCategories> ProductCategories => Set<ProductCategories>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cart> Cart => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (ConnectionString != null)
                    optionsBuilder.UseSqlServer(ConnectionString);
            }
        }
    }
}
