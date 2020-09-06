using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class _20200901ModelTempAddModelOrg_Id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelOrgId",
                table: "ModelTemp",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelOrgId",
                table: "ModelTemp");
        }
    }
}
