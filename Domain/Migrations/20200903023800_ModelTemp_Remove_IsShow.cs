using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class ModelTemp_Remove_IsShow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isShow",
                table: "ModelTemp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "isShow",
                table: "ModelTemp",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
