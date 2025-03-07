using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogNotifications_Karter_AuthorId",
                schema: "dbo",
                table: "BlogNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Upvotes_Karter_KarterId",
                table: "Upvotes");

            migrationBuilder.DropIndex(
                name: "IX_Upvotes_KarterId",
                table: "Upvotes");

            migrationBuilder.DropColumn(
                name: "KarterId",
                table: "Upvotes");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                schema: "dbo",
                table: "BlogNotifications",
                newName: "KarterId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogNotifications_AuthorId",
                schema: "dbo",
                table: "BlogNotifications",
                newName: "IX_BlogNotifications_KarterId");

            migrationBuilder.AddColumn<int>(
                name: "KarterId",
                table: "BlogPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Upvotes_VoterId",
                table: "Upvotes",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_KarterId",
                table: "BlogPosts",
                column: "KarterId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogNotifications_userId",
                schema: "dbo",
                table: "BlogNotifications",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogNotifications_Karter_KarterId",
                schema: "dbo",
                table: "BlogNotifications",
                column: "KarterId",
                principalTable: "Karter",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogNotifications_Karter_userId",
                schema: "dbo",
                table: "BlogNotifications",
                column: "userId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Karter_KarterId",
                table: "BlogPosts",
                column: "KarterId",
                principalTable: "Karter",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Upvotes_Karter_VoterId",
                table: "Upvotes",
                column: "VoterId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogNotifications_Karter_KarterId",
                schema: "dbo",
                table: "BlogNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogNotifications_Karter_userId",
                schema: "dbo",
                table: "BlogNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Karter_KarterId",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Upvotes_Karter_VoterId",
                table: "Upvotes");

            migrationBuilder.DropIndex(
                name: "IX_Upvotes_VoterId",
                table: "Upvotes");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_KarterId",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogNotifications_userId",
                schema: "dbo",
                table: "BlogNotifications");

            migrationBuilder.DropColumn(
                name: "KarterId",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "KarterId",
                schema: "dbo",
                table: "BlogNotifications",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogNotifications_KarterId",
                schema: "dbo",
                table: "BlogNotifications",
                newName: "IX_BlogNotifications_AuthorId");

            migrationBuilder.AddColumn<int>(
                name: "KarterId",
                table: "Upvotes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Upvotes_KarterId",
                table: "Upvotes",
                column: "KarterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogNotifications_Karter_AuthorId",
                schema: "dbo",
                table: "BlogNotifications",
                column: "AuthorId",
                principalTable: "Karter",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Upvotes_Karter_KarterId",
                table: "Upvotes",
                column: "KarterId",
                principalTable: "Karter",
                principalColumn: "Id");
        }
    }
}
