﻿// <auto-generated />
using System;
using GoKartUnite.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GoKartUnite.Migrations
{
    [DbContext(typeof(GoKartUniteContext))]
    [Migration("20241226202453_KarterFollowsTrack")]
    partial class KarterFollowsTrack
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GoKartUnite.Models.BlogPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTimePosted")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descripttion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TaggedTrackId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TaggedTrackId");

                    b.ToTable("BlogPosts");
                });

            modelBuilder.Entity("GoKartUnite.Models.FollowTrack", b =>
                {
                    b.Property<int>("KarterId")
                        .HasColumnType("int");

                    b.Property<int>("TrackId")
                        .HasColumnType("int");

                    b.HasKey("KarterId", "TrackId");

                    b.HasIndex("TrackId");

                    b.ToTable("FollowTracks");
                });

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

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GoogleId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity("GoKartUnite.Models.Upvotes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("KarterId")
                        .HasColumnType("int");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<int>("VoterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("KarterId");

                    b.HasIndex("PostId");

                    b.ToTable("Upvotes");
                });

            modelBuilder.Entity("GoKartUnite.Models.BlogPost", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "Author")
                        .WithMany("BlogPosts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Track", "TaggedTrack")
                        .WithMany("BlogPosts")
                        .HasForeignKey("TaggedTrackId");

                    b.Navigation("Author");

                    b.Navigation("TaggedTrack");
                });

            modelBuilder.Entity("GoKartUnite.Models.FollowTrack", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "karter")
                        .WithMany()
                        .HasForeignKey("KarterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Track", "track")
                        .WithMany()
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("karter");

                    b.Navigation("track");
                });

            modelBuilder.Entity("GoKartUnite.Models.Friendships", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "KarterFirst")
                        .WithMany()
                        .HasForeignKey("KarterFirstId")
                        .OnDelete(DeleteBehavior.Restrict)
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

            modelBuilder.Entity("GoKartUnite.Models.Upvotes", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "Karter")
                        .WithMany()
                        .HasForeignKey("KarterId");

                    b.HasOne("GoKartUnite.Models.BlogPost", "Post")
                        .WithMany("Upvotes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Karter");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("GoKartUnite.Models.BlogPost", b =>
                {
                    b.Navigation("Upvotes");
                });

            modelBuilder.Entity("GoKartUnite.Models.Karter", b =>
                {
                    b.Navigation("BlogPosts");

                    b.Navigation("Friendships");
                });

            modelBuilder.Entity("GoKartUnite.Models.Track", b =>
                {
                    b.Navigation("BlogPosts");

                    b.Navigation("Karters");
                });
#pragma warning restore 612, 618
        }
    }
}
