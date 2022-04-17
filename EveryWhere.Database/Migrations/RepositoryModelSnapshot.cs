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

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .HasComment("上传时间");

                    b.Property<bool>("IsConverted")
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

                    b.Property<int>("PageCount")
                        .HasColumnType("int")
                        .HasColumnName("page_count")
                        .HasComment("页数");

                    b.Property<double>("Size")
                        .HasColumnType("double(8,2)")
                        .HasColumnName("size")
                        .HasComment("文件大小");

                    b.Property<int>("UploaderId")
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

                    b.Property<int>("ConsumerId")
                        .HasColumnType("int(11)")
                        .HasColumnName("consumer_id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(8,2)")
                        .HasColumnName("price")
                        .HasComment("订单价格");

                    b.Property<int>("ShopId")
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

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<bool>("IsWork")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_work");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(120)")
                        .HasColumnName("name")
                        .HasComment("打印机的名称，可由用户更改");

                    b.Property<int>("ShopId")
                        .HasColumnType("int(11)")
                        .HasColumnName("shop_id");

                    b.Property<bool>("SupportColor")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("support_color")
                        .HasComment("打印机是否支持彩色打印");

                    b.Property<bool>("SupportDuplex")
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

                    b.ToTable("printer");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.PrintJob", b =>
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

                    b.Property<int>("FileId")
                        .HasColumnType("int(11)")
                        .HasColumnName("file_id")
                        .HasComment("文件ID");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_finished")
                        .HasComment("是否完成");

                    b.Property<int>("OrderId")
                        .HasColumnType("int(11)")
                        .HasColumnName("order_id")
                        .HasComment("订单ID");

                    b.Property<int>("PrinterId")
                        .HasColumnType("int(11)")
                        .HasColumnName("printer_id")
                        .HasComment("打印机ID");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

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

                    b.HasIndex("ShopKeeperId");

                    b.ToTable("shop");
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

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<bool>("IsManager")
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
                        .WithMany()
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
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EveryWhere.Database.PO.Order", "Order")
                        .WithMany("PrintJobs")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EveryWhere.Database.PO.Printer", "Printer")
                        .WithMany("PrintJobs")
                        .HasForeignKey("PrinterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
