using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StyleONApi.Migrations
{
    public partial class dispatchedit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Dispatchs");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Dispatchs",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Dispatchs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
