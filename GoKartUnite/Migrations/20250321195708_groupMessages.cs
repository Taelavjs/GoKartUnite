using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class groupMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessage_Groups_GroupCommentOnId",
                table: "GroupMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessage_Karter_AuthorId",
                table: "GroupMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMessage",
                table: "GroupMessage");

            migrationBuilder.RenameTable(
                name: "GroupMessage",
                newName: "GroupMessages");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMessage_GroupCommentOnId",
                table: "GroupMessages",
                newName: "IX_GroupMessages_GroupCommentOnId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMessage_AuthorId",
                table: "GroupMessages",
                newName: "IX_GroupMessages_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMessages",
                table: "GroupMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessages_Groups_GroupCommentOnId",
                table: "GroupMessages",
                column: "GroupCommentOnId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessages_Karter_AuthorId",
                table: "GroupMessages",
                column: "AuthorId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessages_Groups_GroupCommentOnId",
                table: "GroupMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessages_Karter_AuthorId",
                table: "GroupMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMessages",
                table: "GroupMessages");

            migrationBuilder.RenameTable(
                name: "GroupMessages",
                newName: "GroupMessage");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMessages_GroupCommentOnId",
                table: "GroupMessage",
                newName: "IX_GroupMessage_GroupCommentOnId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMessages_AuthorId",
                table: "GroupMessage",
                newName: "IX_GroupMessage_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMessage",
                table: "GroupMessage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessage_Groups_GroupCommentOnId",
                table: "GroupMessage",
                column: "GroupCommentOnId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessage_Karter_AuthorId",
                table: "GroupMessage",
                column: "AuthorId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
