using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class TrackLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Location",
                table: "Track",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Track");
        }
    }
}
