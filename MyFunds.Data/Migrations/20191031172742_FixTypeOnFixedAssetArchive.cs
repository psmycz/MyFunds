using Microsoft.EntityFrameworkCore.Migrations;

namespace MyFunds.Data.Migrations
{
    public partial class FixTypeOnFixedAssetArchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "FixedAssetArchives",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "FixedAssetArchives");
        }
    }
}
