﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SmartHomeAPI.DbContexts;

#nullable disable

namespace SmartHomeAPI.Migrations
{
    [DbContext(typeof(SmartHomeDbContext))]
    [Migration("20241019084643_RemovedTwoColumns")]
    partial class RemovedTwoColumns
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SmartHomeAPI.Entities.MeasureDomain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("TopicId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TopicId");

                    b.ToTable("Measurements");
                });

            modelBuilder.Entity("SmartHomeAPI.Entities.TopicDomain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsFavourite")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("SmartHomeAPI.Entities.MeasureDomain", b =>
                {
                    b.HasOne("SmartHomeAPI.Entities.TopicDomain", "Topic")
                        .WithMany("Measurements")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Topic");
                });

            modelBuilder.Entity("SmartHomeAPI.Entities.TopicDomain", b =>
                {
                    b.Navigation("Measurements");
                });
#pragma warning restore 612, 618
        }
    }
}
