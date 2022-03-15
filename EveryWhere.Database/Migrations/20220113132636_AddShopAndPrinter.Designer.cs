﻿// <auto-generated />
using System;
using EveryWhere.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EveryWhere.Database.Migrations
{
    [DbContext(typeof(Repository))]
    [Migration("20220113132636_AddShopAndPrinter")]
    partial class AddShopAndPrinter
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

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
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.Property<string>("OriginalName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("original_name");

                    b.Property<int>("PrintJobId")
                        .HasColumnType("int(11)")
                        .HasColumnName("print_job_id");

                    b.HasKey("Id");

                    b.HasIndex("PrintJobId")
                        .IsUnique();

                    b.ToTable("file");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Order", b =>
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

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("ENUM('NotUploaded','UnPaid','Converting','Printing','NotTaken','Finish')")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("order");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Printer", b =>
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

                    b.Property<string>("MachineGUID")
                        .IsRequired()
                        .HasColumnType("varchar(30)")
                        .HasColumnName("machine_guid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.Property<int>("ShopId")
                        .HasColumnType("int(11)")
                        .HasColumnName("shop_id");

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

                    b.Property<int>("JobSequence")
                        .HasColumnType("int(11)")
                        .HasColumnName("job_sequence");

                    b.Property<int>("OrderId")
                        .HasColumnType("int(11)")
                        .HasColumnName("order_id");

                    b.Property<int>("PrinterId")
                        .HasColumnType("int(11)")
                        .HasColumnName("printer_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("ENUM('NotUploaded','UploadFailed','Uploaded','Converting','Queuing','Printing','NotTaken','Finish')")
                        .HasColumnName("status");

                    b.HasKey("Id");

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

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(30)")
                        .HasColumnName("name");

                    b.Property<DateTime>("UpdateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime")
                        .HasColumnName("update_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("Id");

                    b.ToTable("shop");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.File", b =>
                {
                    b.HasOne("EveryWhere.Database.PO.PrintJob", "PrintJob")
                        .WithOne("File")
                        .HasForeignKey("EveryWhere.Database.PO.File", "PrintJobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PrintJob");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Order", b =>
                {
                    b.HasOne("EveryWhere.Database.PO.Shop", "Shop")
                        .WithMany("Orders")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
                    b.HasOne("EveryWhere.Database.PO.Order", "Order")
                        .WithMany("PrintJobs")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EveryWhere.Database.PO.Printer", "Printer")
                        .WithMany()
                        .HasForeignKey("PrinterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Printer");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Order", b =>
                {
                    b.Navigation("PrintJobs");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.PrintJob", b =>
                {
                    b.Navigation("File");
                });

            modelBuilder.Entity("EveryWhere.Database.PO.Shop", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("Printers");
                });
#pragma warning restore 612, 618
        }
    }
}
