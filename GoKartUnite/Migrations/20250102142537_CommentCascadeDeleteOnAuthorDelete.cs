using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class CommentCascadeDeleteOnAuthorDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Karter_AuthorId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Karter_AuthorId",
                table: "Comments",
                column: "AuthorId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Karter_AuthorId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Karter_AuthorId",
                table: "Comments",
                column: "AuthorId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
