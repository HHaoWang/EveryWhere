using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class ModifyPrintJobCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_print_job_file_file_id",
                table: "print_job");

            migrationBuilder.DropForeignKey(
                name: "FK_print_job_printer_printer_id",
                table: "print_job");

            migrationBuilder.DropIndex(
                name: "IX_print_job_file_id",
                table: "print_job");

            migrationBuilder.AlterColumn<int>(
                name: "printer_id",
                table: "print_job",
                type: "int(11)",
                nullable: true,
                comment: "打印机ID",
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldComment: "打印机ID");

            migrationBuilder.CreateIndex(
                name: "IX_print_job_file_id",
                table: "print_job",
                column: "file_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_print_job_file_file_id",
                table: "print_job",
                column: "file_id",
                principalTable: "file",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_print_job_printer_printer_id",
                table: "print_job",
                column: "printer_id",
                principalTable: "printer",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_print_job_file_file_id",
                table: "print_job");

            migrationBuilder.DropForeignKey(
                name: "FK_print_job_printer_printer_id",
                table: "print_job");

            migrationBuilder.DropIndex(
                name: "IX_print_job_file_id",
                table: "print_job");

            migrationBuilder.AlterColumn<int>(
                name: "printer_id",
                table: "print_job",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                comment: "打印机ID",
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldNullable: true,
                oldComment: "打印机ID");

            migrationBuilder.CreateIndex(
                name: "IX_print_job_file_id",
                table: "print_job",
                column: "file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_print_job_file_file_id",
                table: "print_job",
                column: "file_id",
                principalTable: "file",
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
    }
}
