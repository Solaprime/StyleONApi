using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StyleONApi.Migrations
{
    public partial class ImageObjectAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("f02abcda-21ae-4a81-94ec-bf172c947739"));

            migrationBuilder.CreateTable(
                name: "ImageObject",
                columns: table => new
                {
                    ImageObjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageName = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageObject", x => x.ImageObjectId);
                    table.ForeignKey(
                        name: "FK_ImageObject_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "DatePosted", "Description", "Name", "Price", "Reviews", "SlashPrice" },
                values: new object[] { new Guid("fad3d35a-704b-4b43-bcdb-5c0251e23a8b"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Dior Bag", 0.0, 0.0, 0.0 });

            migrationBuilder.CreateIndex(
                name: "IX_ImageObject_ProductId",
                table: "ImageObject",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageObject");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("fad3d35a-704b-4b43-bcdb-5c0251e23a8b"));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "DatePosted", "Description", "Name", "Price", "Reviews", "SlashPrice" },
                values: new object[] { new Guid("f02abcda-21ae-4a81-94ec-bf172c947739"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Dior Bag", 0.0, 0.0, 0.0 });
        }
    }
}
