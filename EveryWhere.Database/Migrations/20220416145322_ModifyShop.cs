using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class ModifyShop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "shop",
                keyColumn: "tel",
                keyValue: null,
                column: "tel",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tel",
                table: "shop",
                type: "varchar(11)",
                nullable: false,
                comment: "电话号",
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldNullable: true,
                oldComment: "电话号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tel",
                table: "shop",
                type: "varchar(11)",
                nullable: true,
                comment: "电话号",
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldComment: "电话号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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
    }
}
