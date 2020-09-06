using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class _20200901ModelTempAddModelType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempType",
                table: "ModelTemp",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempType",
                table: "ModelTemp");
        }
    }
}
