using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class taggedTrackBlogUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaggedTrackId",
                table: "BlogPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_TaggedTrackId",
                table: "BlogPosts",
                column: "TaggedTrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Track_TaggedTrackId",
                table: "BlogPosts",
                column: "TaggedTrackId",
                principalTable: "Track",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Track_TaggedTrackId",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_TaggedTrackId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "TaggedTrackId",
                table: "BlogPosts");
        }
    }
}
