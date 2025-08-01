using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext : DbContext
{
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options): base(options){ }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Product>(eb =>
        {
            eb.ToTable("Products");
            eb.HasKey(p => p.Id);
            eb.Property(p => p.Id).ValueGeneratedNever();
            eb.Property(p => p.Sku)
                .IsRequired()
                .HasMaxLength(32);
            eb.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(128);
            eb.Property(p => p.InternalCode)
                .IsRequired()
                .HasMaxLength(32);
            eb.Property(p => p.CurrentUnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            eb.Property(p => p.StockQuantity)
                .IsRequired();
        });

        modelBuilder.Entity<Customer>(eb =>
        {
            eb.ToTable("Customers");
            eb.HasKey(c => c.Id);
            eb.Property(c => c.Id).ValueGeneratedNever();
            eb.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(128);
            eb.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(128);
            eb.Property(c => c.PhoneNumber)
                .HasMaxLength(32);
        });

        modelBuilder.Entity<Order>(eb =>
        {
            eb.ToTable("Orders");
            eb.HasKey(o => o.Id);
            eb.Property(o => o.Id).ValueGeneratedNever();
            eb.Property(o => o.ShippingAddress)
                .IsRequired()
                .HasMaxLength(256);
            eb.Property(o => o.BillingAddress)
                .IsRequired()
                .HasMaxLength(256);
            eb.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            eb.Property(o => o.Date)
                .IsRequired();
            eb.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OrderItem>(eb =>
        {
            eb.ToTable("OrderItems");
            eb.HasKey(oi => oi.Id);
            eb.Property(oi => oi.Id).ValueGeneratedNever();
            eb.Property(oi => oi.Quantity)
                .IsRequired();
            eb.Property(oi => oi.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            eb.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            eb.HasOne(oi => oi.Product)
                .WithMany(p => p.Items)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
