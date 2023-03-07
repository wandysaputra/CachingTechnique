﻿// <auto-generated />
using System;
using Domain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Domain.Data.Migrations
{
    [DbContext(typeof(LocalContext))]
    [Migration("20230307121741_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.3");

            modelBuilder.Entity("Domain.Data.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImgUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<int?>("RatingId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RatingId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Domain.Data.Entities.ProductRating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("AggregateRating")
                        .HasColumnType("TEXT");

                    b.Property<int>("NumberOfRatings")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ProductRatings");
                });

            modelBuilder.Entity("Domain.Data.Entities.Product", b =>
                {
                    b.HasOne("Domain.Data.Entities.ProductRating", "Rating")
                        .WithMany()
                        .HasForeignKey("RatingId");

                    b.Navigation("Rating");
                });
#pragma warning restore 612, 618
        }
    }
}
