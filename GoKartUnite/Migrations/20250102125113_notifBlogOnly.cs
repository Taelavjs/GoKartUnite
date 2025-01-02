using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class notifBlogOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "Notifications",
                newName: "BlogID");

            migrationBuilder.AddColumn<int>(
                name: "LinkedPostId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LinkedPostId",
                table: "Notifications",
                column: "LinkedPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_BlogPosts_LinkedPostId",
                table: "Notifications",
                column: "LinkedPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_BlogPosts_LinkedPostId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_LinkedPostId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "LinkedPostId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "BlogID",
                table: "Notifications",
                newName: "type");
        }
    }
}
