using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class ModifyFileSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "size",
                table: "file",
                type: "double(8,2)",
                nullable: false,
                comment: "文件大小",
                oldClrType: typeof(float),
                oldType: "float(8,2)",
                oldComment: "文件大小");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "size",
                table: "file",
                type: "float(8,2)",
                nullable: false,
                comment: "文件大小",
                oldClrType: typeof(double),
                oldType: "double(8,2)",
                oldComment: "文件大小");
        }
    }
}
