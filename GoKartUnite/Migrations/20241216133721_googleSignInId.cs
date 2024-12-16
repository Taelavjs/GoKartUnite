using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class googleSignInId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Karter_KarterFirstId",
                table: "Friendships");

            migrationBuilder.AddColumn<string>(
                name: "GoogleSignInId",
                table: "Karter",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Karter_KarterFirstId",
                table: "Friendships",
                column: "KarterFirstId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Karter_KarterFirstId",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "GoogleSignInId",
                table: "Karter");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Karter_KarterFirstId",
                table: "Friendships",
                column: "KarterFirstId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
