using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Track",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: false),
                    IsVerifiedByAdmin = table.Column<bool>(type: "bit", nullable: false),
                    FormattedGoogleLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GooglePlacesId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Track", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Karter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearsExperience = table.Column<int>(type: "int", nullable: false),
                    TrackId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Karter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Karter_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackAdmin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KarterId = table.Column<int>(type: "int", nullable: false),
                    TrackId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackAdmin_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KarterId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTimePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaggedTrackId = table.Column<int>(type: "int", nullable: true),
                    PostType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPosts_Karter_KarterId",
                        column: x => x.KarterId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPosts_Track_TaggedTrackId",
                        column: x => x.TaggedTrackId,
                        principalTable: "Track",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FollowTracks",
                columns: table => new
                {
                    KarterId = table.Column<int>(type: "int", nullable: false),
                    TrackId = table.Column<int>(type: "int", nullable: false),
                    FollowedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowTracks", x => new { x.KarterId, x.TrackId });
                    table.ForeignKey(
                        name: "FK_FollowTracks_Karter_KarterId",
                        column: x => x.KarterId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowTracks_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    KarterFirstId = table.Column<int>(type: "int", nullable: false),
                    KarterSecondId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateOnly>(type: "date", nullable: true),
                    requestedByInt = table.Column<int>(type: "int", nullable: false),
                    accepted = table.Column<bool>(type: "bit", nullable: false),
                    KarterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => new { x.KarterFirstId, x.KarterSecondId });
                    table.ForeignKey(
                        name: "FK_Friendships_Karter_KarterFirstId",
                        column: x => x.KarterFirstId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendships_Karter_KarterId",
                        column: x => x.KarterId,
                        principalTable: "Karter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Friendships_Karter_KarterSecondId",
                        column: x => x.KarterSecondId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HostId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Karter_HostId",
                        column: x => x.HostId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KarterTrackStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RaceLength = table.Column<int>(type: "int", nullable: false),
                    isChampionshipRace = table.Column<bool>(type: "bit", nullable: false),
                    TrackId = table.Column<int>(type: "int", nullable: false),
                    KarterId = table.Column<int>(type: "int", nullable: false),
                    DateOnlyRecorded = table.Column<DateOnly>(type: "date", nullable: false),
                    WEATHERSTATUS = table.Column<int>(type: "int", nullable: false),
                    TEMPERATURE = table.Column<int>(type: "int", nullable: false),
                    BestLapTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KarterTrackStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KarterTrackStats_Karter_KarterId",
                        column: x => x.KarterId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KarterTrackStats_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KarterId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Karter_KarterId",
                        column: x => x.KarterId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogNotifications",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    isViewed = table.Column<bool>(type: "bit", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlogID = table.Column<int>(type: "int", nullable: false),
                    KarterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogNotifications_BlogPosts_BlogID",
                        column: x => x.BlogID,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogNotifications_Karter_KarterId",
                        column: x => x.KarterId,
                        principalTable: "Karter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BlogNotifications_Karter_userId",
                        column: x => x.userId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    BlogPostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Karter_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Upvotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoterId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Upvotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Upvotes_BlogPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Upvotes_Karter_VoterId",
                        column: x => x.VoterId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTimePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GroupCommentOnId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMessages_Groups_GroupCommentOnId",
                        column: x => x.GroupCommentOnId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMessages_Karter_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    KarterId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    MemberRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => new { x.GroupId, x.KarterId });
                    table.ForeignKey(
                        name: "FK_Memberships_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Memberships_Karter_KarterId",
                        column: x => x.KarterId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupNotifications_KarterTrackStats_StatId",
                        column: x => x.StatId,
                        principalTable: "KarterTrackStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogNotifications_BlogID",
                schema: "dbo",
                table: "BlogNotifications",
                column: "BlogID");

            migrationBuilder.CreateIndex(
                name: "IX_BlogNotifications_KarterId",
                schema: "dbo",
                table: "BlogNotifications",
                column: "KarterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogNotifications_userId",
                schema: "dbo",
                table: "BlogNotifications",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_KarterId",
                table: "BlogPosts",
                column: "KarterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_TaggedTrackId",
                table: "BlogPosts",
                column: "TaggedTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_BlogPostId",
                table: "Comments",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowTracks_TrackId",
                table: "FollowTracks",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_KarterId",
                table: "Friendships",
                column: "KarterId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_KarterSecondId",
                table: "Friendships",
                column: "KarterSecondId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessages_AuthorId",
                table: "GroupMessages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessages_GroupCommentOnId",
                table: "GroupMessages",
                column: "GroupCommentOnId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupNotifications_StatId",
                table: "GroupNotifications",
                column: "StatId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_HostId",
                table: "Groups",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Karter_TrackId",
                table: "Karter",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_KarterTrackStats_KarterId",
                table: "KarterTrackStats",
                column: "KarterId");

            migrationBuilder.CreateIndex(
                name: "IX_KarterTrackStats_TrackId",
                table: "KarterTrackStats",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_KarterId",
                table: "Memberships",
                column: "KarterId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAdmin_TrackId",
                table: "TrackAdmin",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Upvotes_PostId",
                table: "Upvotes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Upvotes_VoterId",
                table: "Upvotes",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_KarterId",
                table: "UserRoles",
                column: "KarterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogNotifications",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "FollowTracks");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "GroupMessages");

            migrationBuilder.DropTable(
                name: "GroupNotifications");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "TrackAdmin");

            migrationBuilder.DropTable(
                name: "Upvotes");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "KarterTrackStats");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Karter");

            migrationBuilder.DropTable(
                name: "Track");
        }
    }
}
