using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class RemoveFK_Order_File : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_file_order_order_id",
                table: "file");

            migrationBuilder.DropIndex(
                name: "IX_file_order_id",
                table: "file");

            migrationBuilder.DropColumn(
                name: "order_id",
                table: "file");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "order_id",
                table: "file",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_file_order_id",
                table: "file",
                column: "order_id");

            migrationBuilder.AddForeignKey(
                name: "FK_file_order_order_id",
                table: "file",
                column: "order_id",
                principalTable: "order",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
