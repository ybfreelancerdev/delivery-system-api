using DeliverySystem.Data.Context;
using DeliverySystem.Data.Helpers;
using DeliverySystem.Data.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(DeliverySystemContext context)
        {
            // Ensure DB created
            await context.Database.MigrateAsync();

            // Seed Admin
            if (!context.Users.Any(x => x.Role == "Admin"))
            {
                var admin = new User
                {
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    PasswordHash = ShaHash.Encrypt("123456"),
                    Role = "Admin",
                    CreatedOn = DateTime.UtcNow
                };

                await context.Users.AddAsync(admin);
            }

            // Seed Normal User
            if (!context.Users.Any(x => x.Email == "user@gmail.com"))
            {
                var user = new User
                {
                    Name = "Test User",
                    Email = "user@gmail.com",
                    PasswordHash = ShaHash.Encrypt("123456"),
                    Role = "User",
                    CreatedOn = DateTime.UtcNow
                };

                await context.Users.AddAsync(user);
            }

            await context.SaveChangesAsync();
        }
    }
}
