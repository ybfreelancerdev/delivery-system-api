using DeliverySystem.Data;
using DeliverySystem.Data.Context;

namespace DeliverySystem.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<DeliverySystemContext>();

            await DbInitializer.SeedAsync(context);
        }
    }
}
