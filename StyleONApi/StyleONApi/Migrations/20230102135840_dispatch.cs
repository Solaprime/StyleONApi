using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StyleONApi.Migrations
{
    public partial class dispatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dispatchs",
                columns: table => new
                {
                    DispatchId = table.Column<Guid>(nullable: false),
                    NumberofCompletedDispatch = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    DateRegistered = table.Column<DateTime>(nullable: false),
                    ApplicationUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispatchs", x => x.DispatchId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dispatchs");
        }
    }
}
