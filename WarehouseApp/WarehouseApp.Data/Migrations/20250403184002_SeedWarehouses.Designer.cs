﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WarehouseApp.Data;

#nullable disable

namespace WarehouseApp.Data.Migrations
{
    [DbContext(typeof(WarehouseDbContext))]
    [Migration("20250403184002_SeedWarehouses")]
    partial class SeedWarehouses
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WarehouseApp.Data.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Client identifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasComment("Client's address");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Client's email address");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasComment("Client's name");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)")
                        .HasComment("Client's phone number");

                    b.HasKey("Id");

                    b.ToTable("Clients", t =>
                        {
                            t.HasComment("Clients in the system");
                        });
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ExportInvoice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Export invoice identifier");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Foreign key to the Client");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2")
                        .HasComment("Date of the export invoice");

                    b.Property<string>("InvoiceNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasComment("Export invoice number");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ExportInvoices", t =>
                        {
                            t.HasComment("Export invoices in the system");
                        });
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ExportInvoiceDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Export invoice detail identifier");

                    b.Property<Guid>("ExportInvoiceId")
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Foreign key to the ExportInvoice");

                    b.Property<Guid>("ImportInvoiceDetailId")
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Foreign key to the ImportInvoiceDetail");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasComment("Quantity of the product in this export invoice detail");

                    b.Property<decimal?>("UnitPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("Unit price of the product in this export invoice detail");

                    b.HasKey("Id");

                    b.HasIndex("ExportInvoiceId");

                    b.HasIndex("ImportInvoiceDetailId");

                    b.ToTable("ExportInvoiceDetails", t =>
                        {
                            t.HasComment("Details about the exported products");
                        });
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ImportInvoice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Import invoice identifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2")
                        .HasComment("Date of the import invoice");

                    b.Property<string>("InvoiceNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasComment("Unique import invoice number per warehouse");

                    b.Property<Guid>("SupplierId")
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Foreign key to the Client");

                    b.Property<Guid>("WarehouseId")
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Foreign key to the Warehouse");

                    b.HasKey("Id");

                    b.HasIndex("SupplierId");

                    b.HasIndex("WarehouseId", "InvoiceNumber")
                        .IsUnique();

                    b.ToTable("ImportInvoices", t =>
                        {
                            t.HasComment("Import invoices in the system");
                        });
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ImportInvoiceDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Import invoice detail identifier");

                    b.Property<Guid>("ImportInvoiceId")
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Foreign key to the ImportInvoice");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Foreign key to the Product");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasComment("Quantity of the product in this import invoice detail");

                    b.Property<decimal?>("UnitPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("Unit price of the product in this import invoice detail, optional");

                    b.HasKey("Id");

                    b.HasIndex("ImportInvoiceId");

                    b.HasIndex("ProductId");

                    b.ToTable("ImportInvoiceDetails", t =>
                        {
                            t.HasComment("Details about the imported products");
                        });
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Product identifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Foreign key to the Category");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasComment("Description of the product, optional");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasComment("Name of the product");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products", t =>
                        {
                            t.HasComment("Products available in the warehouse");
                        });
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.Warehouse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasComment("Warehouse identifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasComment("Warehouse address");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()")
                        .HasComment("Shows the date of when the warehouse record was created");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasComment("Shows if warehouse is deleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasComment("Warehouse name");

                    b.Property<double?>("Size")
                        .HasColumnType("float")
                        .HasComment("Warehouse size in square meters (m²)");

                    b.HasKey("Id");

                    b.ToTable("Warehouses", t =>
                        {
                            t.HasComment("Warehouses in the system");
                        });

                    b.HasData(
                        new
                        {
                            Id = new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"),
                            Address = "Location A",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse A",
                            Size = 4650.0
                        },
                        new
                        {
                            Id = new Guid("be8f00a5-682d-4b43-9734-d3e17078cb52"),
                            Address = "Location B",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse B",
                            Size = 5200.0
                        },
                        new
                        {
                            Id = new Guid("c3e1a7d3-8e44-4f9b-bf8b-1d3f6e7f8d42"),
                            Address = "Location C",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse C",
                            Size = 4500.0
                        },
                        new
                        {
                            Id = new Guid("d5f6b4e8-3b67-4a7d-bc12-f4a9e6c8d351"),
                            Address = "Location D",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse D",
                            Size = 6300.0
                        },
                        new
                        {
                            Id = new Guid("e8a9c5b7-4d23-49fb-a91b-c6e1f2d8b643"),
                            Address = "Location E",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse E",
                            Size = 7100.0
                        },
                        new
                        {
                            Id = new Guid("f1c2d4a6-5b34-42d8-9c12-e3a7b8f6d921"),
                            Address = "Location F",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse F",
                            Size = 5400.0
                        },
                        new
                        {
                            Id = new Guid("a2b3c4d5-6e78-4f9a-bc12-d4e5f6a7b891"),
                            Address = "Location G",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse G",
                            Size = 5900.0
                        },
                        new
                        {
                            Id = new Guid("b4c5d6e7-8f91-42a3-bc12-e3f4a5d6b782"),
                            Address = "Location H",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse H",
                            Size = 4800.0
                        },
                        new
                        {
                            Id = new Guid("c6d7e8f9-1a23-45b4-bc12-d3e4f5a6b891"),
                            Address = "Location I",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse I",
                            Size = 5200.0
                        },
                        new
                        {
                            Id = new Guid("d8e9f1a2-3b45-47c6-bc12-e2f3a4d5b691"),
                            Address = "Location J",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Name = "Warehouse J",
                            Size = 6000.0
                        });
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ExportInvoice", b =>
                {
                    b.HasOne("WarehouseApp.Data.Models.Client", "Client")
                        .WithMany("ExportInvoices")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ExportInvoiceDetail", b =>
                {
                    b.HasOne("WarehouseApp.Data.Models.ExportInvoice", "ExportInvoice")
                        .WithMany("ExportInvoicesDetails")
                        .HasForeignKey("ExportInvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarehouseApp.Data.Models.ImportInvoiceDetail", "ImportInvoiceDetail")
                        .WithMany("ExportInvoicesPerProduct")
                        .HasForeignKey("ImportInvoiceDetailId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ExportInvoice");

                    b.Navigation("ImportInvoiceDetail");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ImportInvoice", b =>
                {
                    b.HasOne("WarehouseApp.Data.Models.Client", "Supplier")
                        .WithMany("ImportInvoices")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WarehouseApp.Data.Models.Warehouse", "Warehouse")
                        .WithMany("ImportInvoices")
                        .HasForeignKey("WarehouseId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Supplier");

                    b.Navigation("Warehouse");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ImportInvoiceDetail", b =>
                {
                    b.HasOne("WarehouseApp.Data.Models.ImportInvoice", "ImportInvoice")
                        .WithMany("ImportInvoicesDetails")
                        .HasForeignKey("ImportInvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarehouseApp.Data.Models.Product", "Product")
                        .WithMany("ImportInvoicesDetails")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ImportInvoice");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.Product", b =>
                {
                    b.HasOne("WarehouseApp.Data.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.Client", b =>
                {
                    b.Navigation("ExportInvoices");

                    b.Navigation("ImportInvoices");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ExportInvoice", b =>
                {
                    b.Navigation("ExportInvoicesDetails");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ImportInvoice", b =>
                {
                    b.Navigation("ImportInvoicesDetails");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.ImportInvoiceDetail", b =>
                {
                    b.Navigation("ExportInvoicesPerProduct");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.Product", b =>
                {
                    b.Navigation("ImportInvoicesDetails");
                });

            modelBuilder.Entity("WarehouseApp.Data.Models.Warehouse", b =>
                {
                    b.Navigation("ImportInvoices");
                });
#pragma warning restore 612, 618
        }
    }
}
