using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class add_RPG_Proc_Report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RPG_Proc_Report",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreateUserId = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    LastUpdateId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RPG_Proc_Report", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RPG_Proc_Report");
        }
    }
}
