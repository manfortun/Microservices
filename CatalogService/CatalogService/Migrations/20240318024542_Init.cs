using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ImageSource = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => new { x.ProductId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ProductCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => new { x.OwnerId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_Purchases_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Clothing & Apparel" },
                    { 2, "Electronics" },
                    { 3, "Home & Kitchen" },
                    { 4, "Health & Beauty" },
                    { 5, "Sports & Outdoors" },
                    { 6, "Books & Media" },
                    { 7, "Toys & Games" },
                    { 8, "Automotive" },
                    { 9, "Pets" },
                    { 10, "Jewelry & Accessories" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImageSource", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Sleek black tee: Style redefined. Elevate your look effortlessly! 🔥 #FashionEssential", "~/images/OIP.jpg", "T-Shirt", 299.0 },
                    { 2, "Unleash limitless power with our latest cellphone innovation!", "~/images/cellphone.jpg", "Cellphone", 13999.0 },
                    { 3, "Unleash precision in the palm of your hand. Elevate your tools with our sleek knife.", "~/images/ec3596459302e2e8e4d586517816a69a.jpg", "Knife", 240.0 },
                    { 4, "Indulge in luxury with our hydrating lotion. Elevate your skincare routine effortlessly.", "~/images/lotion.jpg", "Lotion", 250.0 },
                    { 5, "Step up your game with our stylish rubber shoes. Elevate your look with every stride.", "~/images/rubbershoes.jpg", "Rubber Shoes", 5500.0 },
                    { 6, "Master clean code principles. Robert Martin's essential guide.", "~/images/cleancode.jpg", "Clean Code", 2890.0 },
                    { 7, "Immerse in endless adventures. Explore, create, survive. Minecraft awaits!", "~/images/Minecraft.jpg", "Minecraft", 150.0 },
                    { 8, "Upgrade your cleaning game with our durable fiber cloth.", "~/images/fibrecloth.jpg", "Fibre Cloth", 40.0 },
                    { 9, "Pure nourishment for your pet. Goat's milk: natural goodness.", "~/images/goatsmilk.jpg", "Goat's Milk", 380.0 },
                    { 10, "Elegant luxury, timeless beauty. Elevate your style with 14k gold.", "~/images/necklace.jpg", "14K Gold Necklace", 21500.0 },
                    { 11, "Powerful laptop with high-speed performance. Perfect for work or entertainment on the go.", "~/images/laptop.jpg", "Laptop", 50000.0 },
                    { 12, "Track your fitness, receive notifications, and more, all from your wrist.", "~/images/smartwatch.jpg", "Smartwatch", 9999.9500000000007 },
                    { 13, "Enjoy crisp sound quality and freedom from wires with these wireless earbuds.", "~/images/wirelessearbuds.jpg", "Wireless Earbuds", 3999.9499999999998 },
                    { 14, "Take your music anywhere with this portable Bluetooth speaker.", "~/images/bluetoothspeaker.jpg", "Portable Bluetooth Speaker", 2499.9499999999998 },
                    { 15, "Monitor your health and track your fitness goals with this sleek fitness tracker.", "~/images/fitnesstracker.jpg", "Fitness Tracker", 2995.0 },
                    { 16, "Brew your favorite coffee just the way you like it.", "~/images/coffeemaker.jpg", "Coffee Maker", 6850.5 },
                    { 17, "Gentle on gums, powerful on plaque.", "~/images/electrictoothbrush.jpg", "Electric Toothbrush", 1999.0 },
                    { 18, "Capture every moment with stunning clarity using this digital camera.", "~/images/digitalcamera.jpg", "Digital Camera", 14560.6 },
                    { 19, "Enjoy healthier cooking without sacrificing flavor with this air fryer.", "~/images/airfryer.jpg", "Air Fryer", 4499.0 },
                    { 20, "Never run out of battery again with this portable power bank.", "~/images/powerbank.jpg", "Portable Power Bank", 1499.0 }
                });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "CategoryId", "ProductId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 },
                    { 6, 7 },
                    { 7, 7 },
                    { 8, 8 },
                    { 9, 9 },
                    { 10, 10 },
                    { 2, 11 },
                    { 2, 12 },
                    { 5, 12 },
                    { 10, 12 },
                    { 2, 13 },
                    { 2, 14 },
                    { 2, 15 },
                    { 5, 15 },
                    { 10, 15 },
                    { 2, 16 },
                    { 2, 17 },
                    { 2, 18 },
                    { 2, 19 },
                    { 3, 19 },
                    { 2, 20 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryId",
                table: "ProductCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductId_CategoryId",
                table: "ProductCategories",
                columns: new[] { "ProductId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ProductId",
                table: "Purchases",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
