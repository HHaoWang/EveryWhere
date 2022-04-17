using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class ModifyShopIsOpening : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_opening",
                table: "shop",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                comment: "是否营业",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true,
                oldComment: "是否营业");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_opening",
                table: "shop",
                type: "tinyint(1)",
                nullable: true,
                comment: "是否营业",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldComment: "是否营业");
        }
    }
}
