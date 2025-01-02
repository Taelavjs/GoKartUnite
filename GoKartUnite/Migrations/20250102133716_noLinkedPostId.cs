using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class noLinkedPostId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_BlogPosts_LinkedPostId",
                table: "BlogNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Karter_AuthorId",
                table: "BlogNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "BlogNotifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_LinkedPostId",
                table: "BlogNotifications");

            migrationBuilder.DropColumn(
                name: "LinkedPostId",
                table: "BlogNotifications");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "BlogNotifications",
                newName: "BlogNotifications",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_AuthorId",
                schema: "dbo",
                table: "BlogNotifications",
                newName: "IX_BlogNotifications_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogNotifications",
                schema: "dbo",
                table: "BlogNotifications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BlogNotifications_BlogID",
                schema: "dbo",
                table: "BlogNotifications",
                column: "BlogID");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogNotifications_BlogPosts_BlogID",
                schema: "dbo",
                table: "BlogNotifications",
                column: "BlogID",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogNotifications_Karter_AuthorId",
                schema: "dbo",
                table: "BlogNotifications",
                column: "AuthorId",
                principalTable: "Karter",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogNotifications_BlogPosts_BlogID",
                schema: "dbo",
                table: "BlogNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogNotifications_Karter_AuthorId",
                schema: "dbo",
                table: "BlogNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogNotifications",
                schema: "dbo",
                table: "BlogNotifications");

            migrationBuilder.DropIndex(
                name: "IX_BlogNotifications_BlogID",
                schema: "dbo",
                table: "BlogNotifications");

            migrationBuilder.RenameTable(
                name: "BlogNotifications",
                schema: "dbo",
                newName: "BlogNotifications");

            migrationBuilder.RenameIndex(
                name: "IX_BlogNotifications_AuthorId",
                table: "BlogNotifications",
                newName: "IX_Notifications_AuthorId");

            migrationBuilder.AddColumn<int>(
                name: "LinkedPostId",
                table: "BlogNotifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "BlogNotifications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LinkedPostId",
                table: "BlogNotifications",
                column: "LinkedPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_BlogPosts_LinkedPostId",
                table: "BlogNotifications",
                column: "LinkedPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Karter_AuthorId",
                table: "BlogNotifications",
                column: "AuthorId",
                principalTable: "Karter",
                principalColumn: "Id");
        }
    }
}
