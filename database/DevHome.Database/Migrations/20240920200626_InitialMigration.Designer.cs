﻿// <auto-generated />
using System;
using DevHome.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DevHome.Database.Migrations
{
    [DbContext(typeof(DevHomeDatabaseContext))]
    [Migration("20240920200626_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("DevHome.Database.DatabaseModels.RepositoryManagement.Repository", b =>
                {
                    b.Property<int>("RepositoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConfigurationFileLocation")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.Property<DateTime?>("CreatedUTCDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("datetime()");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RepositoryClonePath")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.Property<string>("RepositoryName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.Property<string>("RepositoryUri")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("");

                    b.Property<Guid?>("SourceControlClassId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue(new Guid("00000000-0000-0000-0000-000000000000"));

                    b.Property<DateTime?>("UpdatedUTCDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("datetime()");

                    b.HasKey("RepositoryId");

                    b.HasIndex("RepositoryName", "RepositoryClonePath")
                        .IsUnique();

                    b.ToTable("Repository", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
