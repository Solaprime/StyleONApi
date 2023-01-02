using Microsoft.EntityFrameworkCore.Migrations;

namespace StyleONApi.Migrations
{
    public partial class dispatchedit1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispatchs_AspNetUsers_Id",
                table: "Dispatchs");

            migrationBuilder.DropIndex(
                name: "IX_Dispatchs_Id",
                table: "Dispatchs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Dispatchs");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Dispatchs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispatchs_UserId",
                table: "Dispatchs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispatchs_AspNetUsers_UserId",
                table: "Dispatchs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispatchs_AspNetUsers_UserId",
                table: "Dispatchs");

            migrationBuilder.DropIndex(
                name: "IX_Dispatchs_UserId",
                table: "Dispatchs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Dispatchs");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Dispatchs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispatchs_Id",
                table: "Dispatchs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispatchs_AspNetUsers_Id",
                table: "Dispatchs",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
