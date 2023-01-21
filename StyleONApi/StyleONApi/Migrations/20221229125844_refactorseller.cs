using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StyleONApi.Migrations
{
    public partial class refactorseller : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sellers_AspNetUsers_UserFlowId",
                table: "Sellers");

            migrationBuilder.DropIndex(
                name: "IX_Sellers_UserFlowId",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "UserFlowId",
                table: "Sellers");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Sellers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Sellers");

            migrationBuilder.AddColumn<string>(
                name: "UserFlowId",
                table: "Sellers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_UserFlowId",
                table: "Sellers",
                column: "UserFlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sellers_AspNetUsers_UserFlowId",
                table: "Sellers",
                column: "UserFlowId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
