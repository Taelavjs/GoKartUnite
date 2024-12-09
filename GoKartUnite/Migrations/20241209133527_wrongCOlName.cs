using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class wrongCOlName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KarterId",
                table: "Friendships",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_KarterId",
                table: "Friendships",
                column: "KarterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Karter_KarterId",
                table: "Friendships",
                column: "KarterId",
                principalTable: "Karter",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Karter_KarterId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_KarterId",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "KarterId",
                table: "Friendships");
        }
    }
}
