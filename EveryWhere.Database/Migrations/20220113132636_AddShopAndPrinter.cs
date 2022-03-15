using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class AddShopAndPrinter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "print_job",
                type: "ENUM('NotUploaded','UploadFailed','Uploaded','Converting','Queuing','Printing','NotTaken','Finish')",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ENUM('NotUploaded','UploadFailed','Converting','Uploaded','Queuing','Printing','NotTaken','Finish')")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "printer_id",
                table: "print_job",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "shop_id",
                table: "order",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "shop",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(30)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "printer",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    machine_guid = table.Column<string>(type: "varchar(30)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    shop_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_printer", x => x.id);
                    table.ForeignKey(
                        name: "FK_printer_shop_shop_id",
                        column: x => x.shop_id,
                        principalTable: "shop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_print_job_printer_id",
                table: "print_job",
                column: "printer_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_shop_id",
                table: "order",
                column: "shop_id");

            migrationBuilder.CreateIndex(
                name: "IX_printer_shop_id",
                table: "printer",
                column: "shop_id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_shop_shop_id",
                table: "order",
                column: "shop_id",
                principalTable: "shop",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_print_job_printer_printer_id",
                table: "print_job",
                column: "printer_id",
                principalTable: "printer",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_shop_shop_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_print_job_printer_printer_id",
                table: "print_job");

            migrationBuilder.DropTable(
                name: "printer");

            migrationBuilder.DropTable(
                name: "shop");

            migrationBuilder.DropIndex(
                name: "IX_print_job_printer_id",
                table: "print_job");

            migrationBuilder.DropIndex(
                name: "IX_order_shop_id",
                table: "order");

            migrationBuilder.DropColumn(
                name: "printer_id",
                table: "print_job");

            migrationBuilder.DropColumn(
                name: "shop_id",
                table: "order");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "print_job",
                type: "ENUM('NotUploaded','UploadFailed','Converting','Uploaded','Queuing','Printing','NotTaken','Finish')",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ENUM('NotUploaded','UploadFailed','Uploaded','Converting','Queuing','Printing','NotTaken','Finish')")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
