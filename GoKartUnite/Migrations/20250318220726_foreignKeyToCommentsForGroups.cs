using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class foreignKeyToCommentsForGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessage_Groups_GroupId",
                table: "GroupMessage");

            migrationBuilder.DropIndex(
                name: "IX_GroupMessage_GroupId",
                table: "GroupMessage");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "GroupMessage");

            migrationBuilder.AddColumn<int>(
                name: "GroupCommentOnId",
                table: "GroupMessage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessage_GroupCommentOnId",
                table: "GroupMessage",
                column: "GroupCommentOnId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessage_Groups_GroupCommentOnId",
                table: "GroupMessage",
                column: "GroupCommentOnId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessage_Groups_GroupCommentOnId",
                table: "GroupMessage");

            migrationBuilder.DropIndex(
                name: "IX_GroupMessage_GroupCommentOnId",
                table: "GroupMessage");

            migrationBuilder.DropColumn(
                name: "GroupCommentOnId",
                table: "GroupMessage");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "GroupMessage",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessage_GroupId",
                table: "GroupMessage",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessage_Groups_GroupId",
                table: "GroupMessage",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
