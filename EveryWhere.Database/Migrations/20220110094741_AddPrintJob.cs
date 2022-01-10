using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class AddPrintJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "job_sequence",
                table: "file",
                newName: "print_job_id");

            migrationBuilder.CreateTable(
                name: "print_job",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    job_sequence = table.Column<int>(type: "int(11)", nullable: false),
                    order_id = table.Column<int>(type: "int(11)", nullable: false),
                    status = table.Column<string>(type: "ENUM('NotUploaded','UploadFailed','Converting','Uploaded','Queuing','Printing','NotTaken','Finish')", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_print_job", x => x.id);
                    table.ForeignKey(
                        name: "FK_print_job_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_file_print_job_id",
                table: "file",
                column: "print_job_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_print_job_order_id",
                table: "print_job",
                column: "order_id");

            migrationBuilder.AddForeignKey(
                name: "FK_file_print_job_print_job_id",
                table: "file",
                column: "print_job_id",
                principalTable: "print_job",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_file_print_job_print_job_id",
                table: "file");

            migrationBuilder.DropTable(
                name: "print_job");

            migrationBuilder.DropIndex(
                name: "IX_file_print_job_id",
                table: "file");

            migrationBuilder.RenameColumn(
                name: "print_job_id",
                table: "file",
                newName: "job_sequence");
        }
    }
}
