using Microsoft.EntityFrameworkCore.Migrations;

namespace MyFunds.Data.Migrations
{
    public partial class FixedAssetArchiveTypeFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "FixedAssetArchives",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "FixedAssetArchives",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
