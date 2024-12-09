﻿// <auto-generated />
using System;
using GoKartUnite.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GoKartUnite.Migrations
{
    [DbContext(typeof(GoKartUniteContext))]
    partial class GoKartUniteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GoKartUnite.Models.Friendships", b =>
                {
                    b.Property<int>("KarterFirstId")
                        .HasColumnType("int");

                    b.Property<int>("KarterSecondId")
                        .HasColumnType("int");

                    b.Property<DateOnly?>("DateCreated")
                        .HasColumnType("date");

                    b.Property<int?>("KarterId")
                        .HasColumnType("int");

                    b.HasKey("KarterFirstId", "KarterSecondId");

                    b.HasIndex("KarterId");

                    b.HasIndex("KarterSecondId");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("GoKartUnite.Models.Karter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<int?>("TrackId")
                        .HasColumnType("int");

                    b.Property<int>("YearsExperience")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TrackId");

                    b.ToTable("Karter");
                });

            modelBuilder.Entity("GoKartUnite.Models.Track", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Location")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Track");
                });

            modelBuilder.Entity("GoKartUnite.Models.Friendships", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "KarterFirst")
                        .WithMany()
                        .HasForeignKey("KarterFirstId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Karter", null)
                        .WithMany("Friendships")
                        .HasForeignKey("KarterId");

                    b.HasOne("GoKartUnite.Models.Karter", "KarterSecond")
                        .WithMany()
                        .HasForeignKey("KarterSecondId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("KarterFirst");

                    b.Navigation("KarterSecond");
                });

            modelBuilder.Entity("GoKartUnite.Models.Karter", b =>
                {
                    b.HasOne("GoKartUnite.Models.Track", "Track")
                        .WithMany("Karters")
                        .HasForeignKey("TrackId");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("GoKartUnite.Models.Karter", b =>
                {
                    b.Navigation("Friendships");
                });

            modelBuilder.Entity("GoKartUnite.Models.Track", b =>
                {
                    b.Navigation("Karters");
                });
#pragma warning restore 612, 618
        }
    }
}
