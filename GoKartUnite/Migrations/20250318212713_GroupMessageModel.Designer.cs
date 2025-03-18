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
    [Migration("20250318212713_GroupMessageModel")]
    partial class GroupMessageModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GoKartUnite.Models.BlogNotifications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BlogID")
                        .HasColumnType("int");

                    b.Property<int?>("KarterId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isViewed")
                        .HasColumnType("bit");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BlogID");

                    b.HasIndex("KarterId");

                    b.HasIndex("userId");

                    b.ToTable("BlogNotifications", "dbo");
                });

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

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("KarterId")
                        .HasColumnType("int");

                    b.Property<int>("PostType")
                        .HasColumnType("int");

                    b.Property<int?>("TaggedTrackId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("KarterId");

                    b.HasIndex("TaggedTrackId");

                    b.ToTable("BlogPosts");
                });

            modelBuilder.Entity("GoKartUnite.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int>("BlogPostId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BlogPostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("GoKartUnite.Models.FollowTrack", b =>
                {
                    b.Property<int>("KarterId")
                        .HasColumnType("int");

                    b.Property<int>("TrackId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FollowedAt")
                        .HasColumnType("datetime2");

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

                    b.Property<bool>("accepted")
                        .HasColumnType("bit");

                    b.Property<int>("requestedByInt")
                        .HasColumnType("int");

                    b.HasKey("KarterFirstId", "KarterSecondId");

                    b.HasIndex("KarterId");

                    b.HasIndex("KarterSecondId");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("GoKartUnite.Models.Groups.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HostId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("HostId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("GoKartUnite.Models.Groups.GroupMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTimePosted")
                        .HasColumnType("datetime2");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("GroupId");

                    b.ToTable("GroupMessage");
                });

            modelBuilder.Entity("GoKartUnite.Models.Groups.Membership", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("KarterId")
                        .HasColumnType("int");

                    b.Property<int>("MemberRole")
                        .HasColumnType("int");

                    b.HasKey("GroupId", "KarterId");

                    b.HasIndex("KarterId");

                    b.ToTable("Memberships");
                });

            modelBuilder.Entity("GoKartUnite.Models.Karter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameIdentifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TrackId")
                        .HasColumnType("int");

                    b.Property<int>("YearsExperience")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TrackId");

                    b.ToTable("Karter");
                });

            modelBuilder.Entity("GoKartUnite.Models.KarterTrackStats", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<TimeSpan>("BestLapTime")
                        .HasColumnType("time");

                    b.Property<DateOnly>("DateOnlyRecorded")
                        .HasColumnType("date");

                    b.Property<int>("KarterId")
                        .HasColumnType("int");

                    b.Property<int>("RaceLength")
                        .HasColumnType("int");

                    b.Property<string>("RaceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TEMPERATURE")
                        .HasColumnType("int");

                    b.Property<int>("TrackId")
                        .HasColumnType("int");

                    b.Property<int>("WEATHERSTATUS")
                        .HasColumnType("int");

                    b.Property<bool>("isChampionshipRace")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("KarterId");

                    b.HasIndex("TrackId");

                    b.ToTable("KarterTrackStats");
                });

            modelBuilder.Entity("GoKartUnite.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Role");
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

            modelBuilder.Entity("GoKartUnite.Models.TrackAdmins", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("KarterId")
                        .HasColumnType("int");

                    b.Property<int>("TrackId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TrackId");

                    b.ToTable("TrackAdmin");
                });

            modelBuilder.Entity("GoKartUnite.Models.Upvotes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<int>("VoterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("VoterId");

                    b.ToTable("Upvotes");
                });

            modelBuilder.Entity("GoKartUnite.Models.UserRoles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("KarterId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("KarterId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("GoKartUnite.Models.BlogNotifications", b =>
                {
                    b.HasOne("GoKartUnite.Models.BlogPost", "LinkedPost")
                        .WithMany()
                        .HasForeignKey("BlogID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Karter", null)
                        .WithMany("Notification")
                        .HasForeignKey("KarterId");

                    b.HasOne("GoKartUnite.Models.Karter", "Author")
                        .WithMany()
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("LinkedPost");
                });

            modelBuilder.Entity("GoKartUnite.Models.BlogPost", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Karter", null)
                        .WithMany("BlogPosts")
                        .HasForeignKey("KarterId");

                    b.HasOne("GoKartUnite.Models.Track", "TaggedTrack")
                        .WithMany("BlogPosts")
                        .HasForeignKey("TaggedTrackId");

                    b.Navigation("Author");

                    b.Navigation("TaggedTrack");
                });

            modelBuilder.Entity("GoKartUnite.Models.Comment", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.BlogPost", "BlogPost")
                        .WithMany("Comments")
                        .HasForeignKey("BlogPostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("BlogPost");
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

            modelBuilder.Entity("GoKartUnite.Models.Groups.Group", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "HostKarter")
                        .WithMany()
                        .HasForeignKey("HostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("HostKarter");
                });

            modelBuilder.Entity("GoKartUnite.Models.Groups.GroupMessage", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Groups.Group", null)
                        .WithMany("GroupPosts")
                        .HasForeignKey("GroupId");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("GoKartUnite.Models.Groups.Membership", b =>
                {
                    b.HasOne("GoKartUnite.Models.Groups.Group", "Group")
                        .WithMany("MemberKarters")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Karter", "User")
                        .WithMany()
                        .HasForeignKey("KarterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GoKartUnite.Models.Karter", b =>
                {
                    b.HasOne("GoKartUnite.Models.Track", "Track")
                        .WithMany("Karters")
                        .HasForeignKey("TrackId");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("GoKartUnite.Models.KarterTrackStats", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "ForKarter")
                        .WithMany("Stats")
                        .HasForeignKey("KarterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Track", "RecordedTrack")
                        .WithMany("KarterStats")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ForKarter");

                    b.Navigation("RecordedTrack");
                });

            modelBuilder.Entity("GoKartUnite.Models.TrackAdmins", b =>
                {
                    b.HasOne("GoKartUnite.Models.Track", "ManagedTrack")
                        .WithMany()
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ManagedTrack");
                });

            modelBuilder.Entity("GoKartUnite.Models.Upvotes", b =>
                {
                    b.HasOne("GoKartUnite.Models.BlogPost", "Post")
                        .WithMany("Upvotes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Karter", "Karter")
                        .WithMany()
                        .HasForeignKey("VoterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Karter");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("GoKartUnite.Models.UserRoles", b =>
                {
                    b.HasOne("GoKartUnite.Models.Karter", "Karter")
                        .WithMany("UserRoles")
                        .HasForeignKey("KarterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoKartUnite.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Karter");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("GoKartUnite.Models.BlogPost", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Upvotes");
                });

            modelBuilder.Entity("GoKartUnite.Models.Groups.Group", b =>
                {
                    b.Navigation("GroupPosts");

                    b.Navigation("MemberKarters");
                });

            modelBuilder.Entity("GoKartUnite.Models.Karter", b =>
                {
                    b.Navigation("BlogPosts");

                    b.Navigation("Friendships");

                    b.Navigation("Notification");

                    b.Navigation("Stats");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("GoKartUnite.Models.Track", b =>
                {
                    b.Navigation("BlogPosts");

                    b.Navigation("KarterStats");

                    b.Navigation("Karters");
                });
#pragma warning restore 612, 618
        }
    }
}
