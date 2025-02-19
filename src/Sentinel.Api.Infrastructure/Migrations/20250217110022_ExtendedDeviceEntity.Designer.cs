﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sentinel.Api.Infrastructure.Persistence;

#nullable disable

namespace Sentinel.Api.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250217110022_ExtendedDeviceEntity")]
    partial class ExtendedDeviceEntity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("DeviceDetailsId")
                        .HasColumnType("int");

                    b.Property<int>("DeviceSecurityId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceDetailsId");

                    b.HasIndex("DeviceSecurityId");

                    b.HasIndex("OrganisationId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.DeviceDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("GraphicsCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InstalledRam")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Manufacturer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Processor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DeviceDetails");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.DeviceDisk", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<bool>("IsOsDisk")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Size")
                        .HasColumnType("float");

                    b.Property<double>("Used")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("DeviceDisks");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.DeviceSecurity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AntispywareEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("AntivirusEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("DomainFirewallEnabled")
                        .HasColumnType("bit");

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("time");

                    b.Property<bool>("IsVirtualmachine")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastAntispywareUpdate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastAntivirusUpdate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastScan")
                        .HasColumnType("datetime2");

                    b.Property<bool>("NISEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("PrivateFirewallEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("PublicFirewallEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("RealTimeProtectionEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("TamperProtectionEnabled")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("DeviceSecurities");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.DeviceSoftware", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("DeviceSoftware");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.Organisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("Hash")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthenticityToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastVerified")
                        .HasColumnType("datetime2");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("TwoFactorToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.Device", b =>
                {
                    b.HasOne("Sentinel.Api.Domain.Entities.DeviceDetails", "DeviceDetails")
                        .WithMany()
                        .HasForeignKey("DeviceDetailsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sentinel.Api.Domain.Entities.DeviceSecurity", "DeviceSecurity")
                        .WithMany()
                        .HasForeignKey("DeviceSecurityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sentinel.Api.Domain.Entities.Organisation", "Organisation")
                        .WithMany("Devices")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeviceDetails");

                    b.Navigation("DeviceSecurity");

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.DeviceDisk", b =>
                {
                    b.HasOne("Sentinel.Api.Domain.Entities.Device", null)
                        .WithMany("Disks")
                        .HasForeignKey("DeviceId");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.DeviceSoftware", b =>
                {
                    b.HasOne("Sentinel.Api.Domain.Entities.Device", null)
                        .WithMany("Software")
                        .HasForeignKey("DeviceId");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.User", b =>
                {
                    b.HasOne("Sentinel.Api.Domain.Entities.Organisation", "Organisation")
                        .WithMany("Users")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.Device", b =>
                {
                    b.Navigation("Disks");

                    b.Navigation("Software");
                });

            modelBuilder.Entity("Sentinel.Api.Domain.Entities.Organisation", b =>
                {
                    b.Navigation("Devices");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
