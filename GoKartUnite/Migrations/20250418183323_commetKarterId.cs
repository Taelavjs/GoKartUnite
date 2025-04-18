using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class commetKarterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KarterId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_KarterId",
                table: "Comments",
                column: "KarterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Karter_KarterId",
                table: "Comments",
                column: "KarterId",
                principalTable: "Karter",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Karter_KarterId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_KarterId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "KarterId",
                table: "Comments");
        }
    }
}
