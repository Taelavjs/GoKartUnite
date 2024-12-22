using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class taggedTrackBlogUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Track_TaggedTrackId",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "TaggedTrackId",
                table: "BlogPosts",
                newName: "TaggedTrackIdId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_TaggedTrackId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_TaggedTrackIdId");

            migrationBuilder.AddColumn<int>(
                name: "TaggedTrackId1",
                table: "BlogPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_TaggedTrackId1",
                table: "BlogPosts",
                column: "TaggedTrackId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Track_TaggedTrackId1",
                table: "BlogPosts",
                column: "TaggedTrackId1",
                principalTable: "Track",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Track_TaggedTrackIdId",
                table: "BlogPosts",
                column: "TaggedTrackIdId",
                principalTable: "Track",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Track_TaggedTrackId1",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Track_TaggedTrackIdId",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_TaggedTrackId1",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "TaggedTrackId1",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "TaggedTrackIdId",
                table: "BlogPosts",
                newName: "TaggedTrackId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_TaggedTrackIdId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_TaggedTrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Track_TaggedTrackId",
                table: "BlogPosts",
                column: "TaggedTrackId",
                principalTable: "Track",
                principalColumn: "Id");
        }
    }
}
