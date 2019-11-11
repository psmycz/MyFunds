using Microsoft.EntityFrameworkCore.Migrations;

namespace MyFunds.Data.Migrations
{
    public partial class NullableFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FixedAssets_AspNetUsers_UserId",
                table: "FixedAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_MobileAssets_AspNetUsers_UserId",
                table: "MobileAssets");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MobileAssets",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MobileAssetArchives",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FixedAssets",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FixedAssetArchives",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_FixedAssets_AspNetUsers_UserId",
                table: "FixedAssets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MobileAssets_AspNetUsers_UserId",
                table: "MobileAssets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FixedAssets_AspNetUsers_UserId",
                table: "FixedAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_MobileAssets_AspNetUsers_UserId",
                table: "MobileAssets");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MobileAssets",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MobileAssetArchives",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FixedAssets",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FixedAssetArchives",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FixedAssets_AspNetUsers_UserId",
                table: "FixedAssets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MobileAssets_AspNetUsers_UserId",
                table: "MobileAssets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
