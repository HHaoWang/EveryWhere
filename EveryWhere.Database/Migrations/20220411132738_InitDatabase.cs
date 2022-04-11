using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nick_name = table.Column<string>(type: "varchar(120)", nullable: false, comment: "昵称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tel = table.Column<string>(type: "varchar(11)", nullable: false, comment: "电话号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wechat_open_id = table.Column<string>(type: "varchar(120)", nullable: false, comment: "微信下发的openId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wechat_union_id = table.Column<string>(type: "varchar(120)", nullable: false, comment: "微信下发的unionId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wechat_session_key = table.Column<string>(type: "varchar(120)", nullable: false, comment: "微信下发的sessionKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_manager = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false, comment: "是否是管理者"),
                    create_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "file",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    create_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "上传时间"),
                    uploader_id = table.Column<int>(type: "int(11)", nullable: false),
                    size = table.Column<float>(type: "float(8,2)", nullable: false, comment: "文件大小"),
                    original_name = table.Column<string>(type: "varchar(120)", nullable: false, comment: "文件上传时的原始名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(120)", nullable: false, comment: "在服务器上的文件名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    location = table.Column<string>(type: "varchar(120)", nullable: false, comment: "存放位置")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    page_count = table.Column<int>(type: "int", nullable: false, comment: "页数"),
                    is_converted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false, comment: "是否转换完成")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file", x => x.id);
                    table.ForeignKey(
                        name: "FK_file_user_uploader_id",
                        column: x => x.uploader_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shop",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", nullable: false, comment: "店铺名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    open_time = table.Column<TimeOnly>(type: "time", nullable: false, defaultValueSql: "'00:00:00'", comment: "开始营业时间"),
                    close_time = table.Column<TimeOnly>(type: "time", nullable: false, defaultValueSql: "'00:00:00'", comment: "结束营业时间"),
                    address = table.Column<string>(type: "varchar(120)", nullable: false, comment: "店铺位置描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area = table.Column<string>(type: "varchar(10)", nullable: false, comment: "店铺名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    location = table.Column<string>(type: "varchar(20)", nullable: true, comment: "经纬度")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    shopkeeper_id = table.Column<int>(type: "int(11)", nullable: false, comment: "店主ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop", x => x.id);
                    table.ForeignKey(
                        name: "FK_shop_user_shopkeeper_id",
                        column: x => x.shopkeeper_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    create_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    consumer_id = table.Column<int>(type: "int(11)", nullable: false),
                    shop_id = table.Column<int>(type: "int(11)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(8,2)", nullable: false, comment: "订单价格"),
                    state = table.Column<string>(type: "enum('UnPaid','Printing','Finished')", nullable: false, defaultValueSql: "'UnPaid'", comment: "订单状态")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_shop_shop_id",
                        column: x => x.shop_id,
                        principalTable: "shop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_user_consumer_id",
                        column: x => x.consumer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "printer",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", nullable: false, comment: "打印机的名称，可由用户更改")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shop_id = table.Column<int>(type: "int(11)", nullable: false),
                    is_work = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    support_color = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "打印机是否支持彩色打印"),
                    support_duplex = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "打印机是否支持双面打印"),
                    computer_id = table.Column<string>(type: "varchar(40)", nullable: false, comment: "计算机标识")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    support_sizes = table.Column<string>(type: "json", nullable: false, comment: "支持的纸张大小")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
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

            migrationBuilder.CreateTable(
                name: "print_job",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_finished = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否完成"),
                    file_id = table.Column<int>(type: "int(11)", nullable: false, comment: "文件ID"),
                    order_id = table.Column<int>(type: "int(11)", nullable: false, comment: "订单ID"),
                    printer_id = table.Column<int>(type: "int(11)", nullable: false, comment: "打印机ID"),
                    create_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_print_job", x => x.id);
                    table.ForeignKey(
                        name: "FK_print_job_file_file_id",
                        column: x => x.file_id,
                        principalTable: "file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_print_job_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_print_job_printer_printer_id",
                        column: x => x.printer_id,
                        principalTable: "printer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_file_uploader_id",
                table: "file",
                column: "uploader_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_consumer_id",
                table: "order",
                column: "consumer_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_shop_id",
                table: "order",
                column: "shop_id");

            migrationBuilder.CreateIndex(
                name: "IX_print_job_file_id",
                table: "print_job",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_print_job_order_id",
                table: "print_job",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_print_job_printer_id",
                table: "print_job",
                column: "printer_id");

            migrationBuilder.CreateIndex(
                name: "IX_printer_shop_id",
                table: "printer",
                column: "shop_id");

            migrationBuilder.CreateIndex(
                name: "IX_shop_shopkeeper_id",
                table: "shop",
                column: "shopkeeper_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "print_job");

            migrationBuilder.DropTable(
                name: "file");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "printer");

            migrationBuilder.DropTable(
                name: "shop");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
