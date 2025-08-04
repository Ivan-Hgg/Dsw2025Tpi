using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace Dsw2025Tpi.Data.Helpers;

public static class DbContextExtensions
{
    private static readonly JsonSerializerOptions CachedJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "ADMINISTRADOR", "CLIENTE"};
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static void SeedDatabase(this Dsw2025TpiContext context)
    {
        // 1. Customers
        if (!context.Customers.Any())
        {
            var customersJson = File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Dsw2025Tpi.Data", "Sources", "customers.json"));
            var customers = JsonSerializer.Deserialize<List<Customer>>(customersJson, CachedJsonOptions);
            if (customers != null && customers.Count > 0)
            {
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }
        }

        // 2. Products
        if (!context.Products.Any())
        {
            var productsJson = File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Dsw2025Tpi.Data", "Sources", "products.json"));
            var products = JsonSerializer.Deserialize<List<Product>>(productsJson, CachedJsonOptions);
            if (products != null && products.Count > 0)
            {
                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }

        // 3. Orders (sin OrderItems)
        if (!context.Orders.Any())
        {
            var ordersJson = File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Dsw2025Tpi.Data", "Sources", "orders.json"));
            var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson, CachedJsonOptions);
            if (orders != null && orders.Count > 0)
            {
                // Limpia los OrderItems para evitar problemas de navegación
                foreach (var order in orders)
                {
                    order.OrderItems = new List<OrderItem>();
                }
                context.Orders.AddRange(orders);
                context.SaveChanges();
            }
        }

        // 4. OrderItems
        if (!context.OrderItems.Any())
        {
            var orderItemsJson = File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Dsw2025Tpi.Data", "Sources", "orderItems.json"));
            var orderItems = JsonSerializer.Deserialize<List<OrderItem>>(orderItemsJson, CachedJsonOptions);
            if (orderItems != null && orderItems.Count > 0)
            {
                context.OrderItems.AddRange(orderItems);
                context.SaveChanges();
            }
        }
    }
}
