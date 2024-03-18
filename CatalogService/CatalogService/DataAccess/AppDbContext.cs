using CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DataAccess;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Purchase> Purchases { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        Category[] seedCategories =
        [
            new() { Id = 1, Name = "Clothing & Apparel" },
            new() { Id = 2, Name = "Electronics" },
            new() { Id = 3, Name = "Home & Kitchen" },
            new() { Id = 4, Name = "Health & Beauty" },
            new() { Id = 5, Name = "Sports & Outdoors" },
            new() { Id = 6, Name = "Books & Media" },
            new() { Id = 7, Name = "Toys & Games" },
            new() { Id = 8, Name = "Automotive" },
            new() { Id = 9, Name = "Pets" },
            new() { Id = 10, Name = "Jewelry & Accessories" }
        ];

        Product[] seedProducts =
        [
            new() { Id = 1, Name = "T-Shirt", Price = 299.00, ImageSource="~/images/OIP.jpg", Description = "Sleek black tee: Style redefined. Elevate your look effortlessly! 🔥 #FashionEssential" },
            new() { Id = 2, Name = "Cellphone", Price = 13999.00, ImageSource="~/images/cellphone.jpg", Description = "Unleash limitless power with our latest cellphone innovation!" },
            new() { Id = 3, Name = "Knife", Price = 240.00, ImageSource="~/images/ec3596459302e2e8e4d586517816a69a.jpg", Description = "Unleash precision in the palm of your hand. Elevate your tools with our sleek knife." },
            new() { Id = 4, Name = "Lotion", Price = 250.00, ImageSource="~/images/lotion.jpg", Description = "Indulge in luxury with our hydrating lotion. Elevate your skincare routine effortlessly." },
            new() { Id = 5, Name = "Rubber Shoes", Price = 5500.00, ImageSource="~/images/rubbershoes.jpg", Description = "Step up your game with our stylish rubber shoes. Elevate your look with every stride." },
            new() { Id = 6, Name = "Clean Code", Price = 2890.00, ImageSource="~/images/cleancode.jpg", Description = "Master clean code principles. Robert Martin's essential guide." },
            new() { Id = 7, Name = "Minecraft", Price = 150.00, ImageSource="~/images/Minecraft.jpg", Description = "Immerse in endless adventures. Explore, create, survive. Minecraft awaits!" },
            new() { Id = 8, Name = "Fibre Cloth", Price = 40.00, ImageSource="~/images/fibrecloth.jpg", Description = "Upgrade your cleaning game with our durable fiber cloth." },
            new() { Id = 9, Name = "Goat's Milk", Price = 380.00, ImageSource="~/images/goatsmilk.jpg", Description = "Pure nourishment for your pet. Goat's milk: natural goodness." },
            new() { Id = 10, Name = "14K Gold Necklace", Price = 21500.00, ImageSource="~/images/necklace.jpg", Description = "Elegant luxury, timeless beauty. Elevate your style with 14k gold." },
            new() { Id = 11, Name = "Laptop", Price = 50000.0, ImageSource="~/images/laptop.jpg", Description = "Powerful laptop with high-speed performance. Perfect for work or entertainment on the go." },
            new() { Id = 12, Name = "Smartwatch", Price = 9999.95, ImageSource="~/images/smartwatch.jpg", Description = "Track your fitness, receive notifications, and more, all from your wrist." },
            new() { Id = 13, Name = "Wireless Earbuds", Price = 3999.95, ImageSource="~/images/wirelessearbuds.jpg", Description = "Enjoy crisp sound quality and freedom from wires with these wireless earbuds." },
            new() { Id = 14, Name = "Portable Bluetooth Speaker", Price = 2499.95, ImageSource="~/images/bluetoothspeaker.jpg", Description = "Take your music anywhere with this portable Bluetooth speaker." },
            new() { Id = 15, Name = "Fitness Tracker", Price = 2995.00, ImageSource="~/images/fitnesstracker.jpg", Description = "Monitor your health and track your fitness goals with this sleek fitness tracker." },
            new() { Id = 16, Name = "Coffee Maker", Price = 6850.50, ImageSource="~/images/coffeemaker.jpg", Description = "Brew your favorite coffee just the way you like it." },
            new() { Id = 17, Name = "Electric Toothbrush", Price = 1999.0, ImageSource="~/images/electrictoothbrush.jpg", Description = "Gentle on gums, powerful on plaque." },
            new() { Id = 18, Name = "Digital Camera", Price = 14560.60, ImageSource="~/images/digitalcamera.jpg", Description = "Capture every moment with stunning clarity using this digital camera." },
            new() { Id = 19, Name = "Air Fryer", Price = 4499.0, ImageSource="~/images/airfryer.jpg", Description = "Enjoy healthier cooking without sacrificing flavor with this air fryer." },
            new() { Id = 20, Name = "Portable Power Bank", Price = 1499.0, ImageSource="~/images/powerbank.jpg", Description = "Never run out of battery again with this portable power bank." },
        ];

        ProductCategory[] seedProductCategories =
        [
            new() { ProductId = 1, CategoryId = 1 },
            new() { ProductId = 2, CategoryId = 2 },
            new() { ProductId = 3, CategoryId = 3 },
            new() { ProductId = 4, CategoryId = 4 },
            new() { ProductId = 5, CategoryId = 5 },
            new() { ProductId = 6, CategoryId = 6 },
            new() { ProductId = 7, CategoryId = 7 },
            new() { ProductId = 7, CategoryId = 6 },
            new() { ProductId = 8, CategoryId = 8 },
            new() { ProductId = 9, CategoryId = 9 },
            new() { ProductId = 10, CategoryId = 10 },
            new() { ProductId = 11, CategoryId = 2 },
            new() { ProductId = 12, CategoryId = 2 },
            new() { ProductId = 12, CategoryId = 5 },
            new() { ProductId = 12, CategoryId = 10 },
            new() { ProductId = 13, CategoryId = 2 },
            new() { ProductId = 14, CategoryId = 2 },
            new() { ProductId = 15, CategoryId = 2 },
            new() { ProductId = 15, CategoryId = 5 },
            new() { ProductId = 15, CategoryId = 10 },
            new() { ProductId = 16, CategoryId = 2 },
            new() { ProductId = 17, CategoryId = 2 },
            new() { ProductId = 18, CategoryId = 2 },
            new() { ProductId = 19, CategoryId = 2 },
            new() { ProductId = 19, CategoryId = 3 },
            new() { ProductId = 20, CategoryId = 2 },
        ];

        builder.Entity<ProductCategory>()
            .HasKey(pc => new { pc.ProductId, pc.CategoryId });

        builder.Entity<ProductCategory>()
            .HasIndex(pc => new { pc.ProductId, pc.CategoryId })
            .IsUnique();

        builder.Entity<Purchase>()
            .HasKey(pc => new { pc.OwnerId, pc.ProductId });

        builder.Entity<Category>().HasData(seedCategories);

        builder.Entity<Product>().HasData(seedProducts);

        builder.Entity<ProductCategory>().HasData(seedProductCategories);
    }
}
