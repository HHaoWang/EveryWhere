using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class ModifyArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "parent_area_id",
                table: "area",
                type: "int(11)",
                nullable: true,
                comment: "上级区划ID");

            migrationBuilder.CreateIndex(
                name: "IX_area_parent_area_id",
                table: "area",
                column: "parent_area_id");

            migrationBuilder.AddForeignKey(
                name: "FK_area_area_parent_area_id",
                table: "area",
                column: "parent_area_id",
                principalTable: "area",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_area_area_parent_area_id",
                table: "area");

            migrationBuilder.DropIndex(
                name: "IX_area_parent_area_id",
                table: "area");

            migrationBuilder.DropColumn(
                name: "parent_area_id",
                table: "area");
        }
    }
}
