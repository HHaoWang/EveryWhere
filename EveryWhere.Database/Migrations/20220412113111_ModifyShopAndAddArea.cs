using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class ModifyShopAndAddArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "shop",
                type: "varchar(10)",
                nullable: false,
                comment: "所在行政区域",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldComment: "店铺名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "tel",
                table: "shop",
                type: "varchar(11)",
                nullable: true,
                comment: "电话号")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "area",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    area_code = table.Column<string>(type: "varchar(10)", nullable: false, comment: "行政区名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(120)", nullable: false, comment: "行政区名称")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_area", x => x.id);
                    table.UniqueConstraint("AK_area_area_code", x => x.area_code);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_shop_area",
                table: "shop",
                column: "area");

            migrationBuilder.CreateIndex(
                name: "IX_area_area_code",
                table: "area",
                column: "area_code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_shop_area_area",
                table: "shop",
                column: "area",
                principalTable: "area",
                principalColumn: "area_code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shop_area_area",
                table: "shop");

            migrationBuilder.DropTable(
                name: "area");

            migrationBuilder.DropIndex(
                name: "IX_shop_area",
                table: "shop");

            migrationBuilder.DropColumn(
                name: "tel",
                table: "shop");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "shop",
                type: "varchar(10)",
                nullable: false,
                comment: "店铺名称",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldComment: "所在行政区域")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
