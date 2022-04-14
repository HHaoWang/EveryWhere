using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class ModifyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "wechat_union_id",
                table: "user",
                type: "varchar(120)",
                nullable: true,
                comment: "微信下发的unionId",
                oldClrType: typeof(string),
                oldType: "varchar(120)",
                oldComment: "微信下发的unionId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "wechat_union_id",
                keyValue: null,
                column: "wechat_union_id",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "wechat_union_id",
                table: "user",
                type: "varchar(120)",
                nullable: false,
                comment: "微信下发的unionId",
                oldClrType: typeof(string),
                oldType: "varchar(120)",
                oldNullable: true,
                oldComment: "微信下发的unionId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
