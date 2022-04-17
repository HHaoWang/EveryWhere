using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class ModifyPrinterOrderAndPrintJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "support_sizes",
                table: "printer");

            migrationBuilder.AlterColumn<int>(
                name: "shop_id",
                table: "printer",
                type: "int(11)",
                nullable: false,
                comment: "店铺ID",
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AlterColumn<bool>(
                name: "is_work",
                table: "printer",
                type: "tinyint(1)",
                nullable: false,
                comment: "当前是否工作",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AddColumn<string>(
                name: "device_name",
                table: "printer",
                type: "varchar(120)",
                nullable: false,
                defaultValue: "",
                comment: "打印机在计算机中的物理名称")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "color",
                table: "print_job",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                comment: "彩色打印");

            migrationBuilder.AddColumn<int>(
                name: "count",
                table: "print_job",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                comment: "打印份数");

            migrationBuilder.AddColumn<bool>(
                name: "duplex",
                table: "print_job",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                comment: "双面打印");

            migrationBuilder.AddColumn<string>(
                name: "fetch_code",
                table: "print_job",
                type: "varchar(10)",
                nullable: true,
                comment: "取件码")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "page_end",
                table: "print_job",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                comment: "打印结束页");

            migrationBuilder.AddColumn<string>(
                name: "page_size",
                table: "print_job",
                type: "varchar(30)",
                nullable: false,
                defaultValue: "",
                comment: "打印纸张大小")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "page_start",
                table: "print_job",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                comment: "打印开始页");

            migrationBuilder.AddColumn<DateTime>(
                name: "complete_time",
                table: "order",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "pay_time",
                table: "order",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "page_count",
                table: "file",
                type: "int",
                nullable: true,
                comment: "页数",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "页数");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "device_name",
                table: "printer");

            migrationBuilder.DropColumn(
                name: "color",
                table: "print_job");

            migrationBuilder.DropColumn(
                name: "count",
                table: "print_job");

            migrationBuilder.DropColumn(
                name: "duplex",
                table: "print_job");

            migrationBuilder.DropColumn(
                name: "fetch_code",
                table: "print_job");

            migrationBuilder.DropColumn(
                name: "page_end",
                table: "print_job");

            migrationBuilder.DropColumn(
                name: "page_size",
                table: "print_job");

            migrationBuilder.DropColumn(
                name: "page_start",
                table: "print_job");

            migrationBuilder.DropColumn(
                name: "complete_time",
                table: "order");

            migrationBuilder.DropColumn(
                name: "pay_time",
                table: "order");

            migrationBuilder.AlterColumn<int>(
                name: "shop_id",
                table: "printer",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldComment: "店铺ID");

            migrationBuilder.AlterColumn<bool>(
                name: "is_work",
                table: "printer",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldComment: "当前是否工作");

            migrationBuilder.AddColumn<string>(
                name: "support_sizes",
                table: "printer",
                type: "json",
                nullable: false,
                comment: "支持的纸张大小")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "page_count",
                table: "file",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "页数",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "页数");
        }
    }
}
