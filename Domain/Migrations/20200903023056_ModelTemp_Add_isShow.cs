using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class ModelTemp_Add_isShow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "isShow",
                table: "ModelTemp",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isShow",
                table: "ModelTemp");
        }
    }
}
