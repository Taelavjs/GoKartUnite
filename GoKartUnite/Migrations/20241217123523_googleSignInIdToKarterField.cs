using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class googleSignInIdToKarterField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "Karter",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "Karter");
        }
    }
}
