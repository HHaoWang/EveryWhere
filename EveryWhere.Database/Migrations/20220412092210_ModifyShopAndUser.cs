using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class ModifyShopAndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tel",
                table: "user",
                type: "varchar(11)",
                nullable: true,
                comment: "电话号",
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldComment: "电话号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "avatar",
                table: "user",
                type: "varchar(120)",
                nullable: false,
                defaultValue: "",
                comment: "头像")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "is_opening",
                table: "shop",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                comment: "是否营业");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar",
                table: "user");

            migrationBuilder.DropColumn(
                name: "is_opening",
                table: "shop");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "tel",
                keyValue: null,
                column: "tel",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tel",
                table: "user",
                type: "varchar(11)",
                nullable: false,
                comment: "电话号",
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldNullable: true,
                oldComment: "电话号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
