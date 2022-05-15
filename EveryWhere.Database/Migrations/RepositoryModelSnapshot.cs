﻿// <auto-generated />
using System;
using EveryWhere.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    [DbContext(typeof(Repository))]
    partial class RepositoryModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EveryWhere.Database.PO.Area", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<string>("AreaCode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasColumnName("area_code")
                        .HasComment("行政区名称");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("name")
                        .HasComment("行政区名称");

                    b.Property<int?>("ParentAreaId")
                        .HasColumnType("int(11)")
                        .HasColumnName("parent_area_id")
                        .HasComment("上级区划ID");

                    b.HasKey("Id");

                    b.HasIndex("AreaCode")
                        .IsUnique();

                    b.HasIndex("ParentAreaId");

                    b.ToTable("area");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CreateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .HasComment("上传时间");

                    b.Property<bool?>("IsConverted")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false)
                        .HasColumnName("is_converted")
                        .HasComment("是否转换完成");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("location")
                        .HasComment("存放位置");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("name")
                        .HasComment("在服务器上的文件名");

                    b.Property<string>("OriginalName")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("original_name")
                        .HasComment("文件上传时的原始名称");

                    b.Property<int?>("PageCount")
                        .HasColumnType("int")
                        .HasColumnName("page_count")
                        .HasComment("页数");

                    b.Property<double?>("Size")
                        .IsRequired()
                        .HasColumnType("double(8,2)")
                        .HasColumnName("size")
                        .HasComment("文件大小");

                    b.Property<int?>("UploaderId")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("uploader_id");

                    b.HasKey("Id");

                    b.HasIndex("UploaderId");

                    b.ToTable("file");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CompleteTime")
                        .HasColumnType("datetime")
                        .HasColumnName("complete_time");

                    b.Property<int?>("ConsumerId")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("consumer_id");

                    b.Property<DateTime?>("CreateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime?>("PayTime")
                        .HasColumnType("datetime")
                        .HasColumnName("pay_time");

                    b.Property<decimal?>("Price")
                        .IsRequired()
                        .HasColumnType("decimal(8,2)")
                        .HasColumnName("price")
                        .HasComment("订单价格");

                    b.Property<int?>("ShopId")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("shop_id");

                    b.Property<string>("State")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("enum('UnPaid','Printing','Finished')")
                        .HasColumnName("state")
                        .HasDefaultValueSql("'UnPaid'")
                        .HasComment("订单状态");

                    b.HasKey("Id");

                    b.HasIndex("ConsumerId");

                    b.HasIndex("ShopId");

                    b.ToTable("order");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Printer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<string>("ComputerId")
                        .IsRequired()
                        .HasColumnType("varchar(40)")
                        .HasColumnName("computer_id")
                        .HasComment("计算机标识");

                    b.Property<DateTime?>("CreateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("DeviceName")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("device_name")
                        .HasComment("打印机在计算机中的物理名称");

                    b.Property<bool?>("IsWork")
                        .IsRequired()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_work")
                        .HasComment("当前是否工作");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("name")
                        .HasComment("打印机的名称，可由用户更改");

                    b.Property<int?>("ShopId")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("shop_id")
                        .HasComment("店铺ID");

                    b.Property<bool?>("SupportColor")
                        .IsRequired()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("support_color")
                        .HasComment("打印机是否支持彩色打印");

                    b.Property<bool?>("SupportDuplex")
                        .IsRequired()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("support_duplex")
                        .HasComment("打印机是否支持双面打印");

                    b.Property<string>("SupportSizesJson")
                        .IsRequired()
                        .HasColumnType("json")
                        .HasColumnName("support_sizes")
                        .HasComment("支持的纸张大小");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.HasIndex("DeviceName", "ComputerId")
                        .IsUnique();

                    b.ToTable("printer");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.PrintJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<bool?>("Color")
                        .IsRequired()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("color")
                        .HasComment("彩色打印");

                    b.Property<int?>("Count")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("count")
                        .HasComment("打印份数");

                    b.Property<DateTime?>("CreateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<bool?>("Duplex")
                        .IsRequired()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("duplex")
                        .HasComment("双面打印");

                    b.Property<string>("FetchCode")
                        .HasColumnType("varchar(10)")
                        .HasColumnName("fetch_code")
                        .HasComment("取件码");

                    b.Property<int?>("FileId")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("file_id")
                        .HasComment("文件ID");

                    b.Property<bool?>("IsFinished")
                        .IsRequired()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_finished")
                        .HasComment("是否完成");

                    b.Property<int?>("OrderId")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("order_id")
                        .HasComment("订单ID");

                    b.Property<int?>("PageEnd")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("page_end")
                        .HasComment("打印结束页");

                    b.Property<string>("PageSize")
                        .IsRequired()
                        .HasColumnType("varchar(30)")
                        .HasColumnName("page_size")
                        .HasComment("打印纸张大小");

                    b.Property<int?>("PageStart")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("page_start")
                        .HasComment("打印开始页");

                    b.Property<decimal?>("Price")
                        .IsRequired()
                        .HasColumnType("decimal(8,2)")
                        .HasColumnName("price")
                        .HasComment("任务价格");

                    b.Property<int?>("PrinterId")
                        .HasColumnType("int(11)")
                        .HasColumnName("printer_id")
                        .HasComment("打印机ID");

                    b.HasKey("Id");

                    b.HasIndex("FileId")
                        .IsUnique();

                    b.HasIndex("OrderId");

                    b.HasIndex("PrinterId");

                    b.ToTable("print_job");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Shop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("address")
                        .HasComment("店铺位置描述");

                    b.Property<string>("AreaCode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasColumnName("area")
                        .HasComment("所在行政区域");

                    b.Property<TimeOnly?>("CloseTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("time")
                        .HasColumnName("close_time")
                        .HasDefaultValueSql("'00:00:00'")
                        .HasComment("结束营业时间");

                    b.Property<DateTime?>("CreateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<bool?>("IsOpening")
                        .IsRequired()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_opening")
                        .HasComment("是否营业");

                    b.Property<string>("Location")
                        .HasColumnType("varchar(20)")
                        .HasColumnName("location")
                        .HasComment("经纬度");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("name")
                        .HasComment("店铺名称");

                    b.Property<TimeOnly?>("OpenTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("time")
                        .HasColumnName("open_time")
                        .HasDefaultValueSql("'00:00:00'")
                        .HasComment("开始营业时间");

                    b.Property<int?>("ShopKeeperId")
                        .IsRequired()
                        .HasColumnType("int(11)")
                        .HasColumnName("shopkeeper_id")
                        .HasComment("店主ID");

                    b.Property<string>("Tel")
                        .IsRequired()
                        .HasColumnType("varchar(11)")
                        .HasColumnName("tel")
                        .HasComment("电话号");

                    b.HasKey("Id");

                    b.HasIndex("AreaCode");

                    b.HasIndex("ShopKeeperId")
                        .IsUnique();

                    b.ToTable("shop");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.ShopView", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("ShopId")
                        .HasColumnType("int(11)")
                        .HasColumnName("shop_id");

                    b.HasKey("Id");

                    b.ToTable("shop_view");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("avatar")
                        .HasComment("头像");

                    b.Property<DateTime?>("CreateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<bool?>("IsManager")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false)
                        .HasColumnName("is_manager")
                        .HasComment("是否是管理者");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("nick_name")
                        .HasComment("昵称");

                    b.Property<string>("Tel")
                        .HasColumnType("varchar(11)")
                        .HasColumnName("tel")
                        .HasComment("电话号");

                    b.Property<string>("WechatOpenId")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("wechat_open_id")
                        .HasComment("微信下发的openId");

                    b.Property<string>("WechatSessionKey")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("wechat_session_key")
                        .HasComment("微信下发的sessionKey");

                    b.Property<string>("WechatUnionId")
                        .HasColumnType("varchar(120)")
                        .HasColumnName("wechat_union_id")
                        .HasComment("微信下发的unionId");

                    b.HasKey("Id");

                    b.HasIndex("WechatOpenId")
                        .IsUnique();

                    b.ToTable("user");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Area", b =>
                {
                    b.HasOne("EveryWhere.Database.PO.Area", "ParentArea")
                        .WithMany("SubAreas")
                        .HasForeignKey("ParentAreaId");

                    b.Navigation("ParentArea");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.File", b =>
                {
                    b.HasOne("EveryWhere.Database.PO.User", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Order", b =>
                {
                    b.HasOne("EveryWhere.Database.PO.User", "Consumer")
                        .WithMany("Orders")
                        .HasForeignKey("ConsumerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EveryWhere.Database.PO.Shop", "Shop")
                        .WithMany("Orders")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Consumer");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Printer", b =>
                {
                    b.HasOne("EveryWhere.Database.PO.Shop", "Shop")
                        .WithMany("Printers")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.PrintJob", b =>
                {
                    b.HasOne("EveryWhere.Database.PO.File", "File")
                        .WithOne("PrintJob")
                        .HasForeignKey("EveryWhere.Database.PO.PrintJob", "FileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EveryWhere.Database.PO.Order", "Order")
                        .WithMany("PrintJobs")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EveryWhere.Database.PO.Printer", "Printer")
                        .WithMany("PrintJobs")
                        .HasForeignKey("PrinterId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("File");

                    b.Navigation("Order");

                    b.Navigation("Printer");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Shop", b =>
                {
                    b.HasOne("EveryWhere.Database.PO.Area", "Area")
                        .WithMany("Shops")
                        .HasForeignKey("AreaCode")
                        .HasPrincipalKey("AreaCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EveryWhere.Database.PO.User", "Shopkeeper")
                        .WithMany()
                        .HasForeignKey("ShopKeeperId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");

                    b.Navigation("Shopkeeper");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Area", b =>
                {
                    b.Navigation("Shops");

                    b.Navigation("SubAreas");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.File", b =>
                {
                    b.Navigation("PrintJob");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Order", b =>
                {
                    b.Navigation("PrintJobs");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Printer", b =>
                {
                    b.Navigation("PrintJobs");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Shop", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("Printers");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
