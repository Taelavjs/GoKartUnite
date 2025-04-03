using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class TracksVerificationFieldAddedToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormattedGoogleLocation",
                table: "Track",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GooglePlacesId",
                table: "Track",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedByAdmin",
                table: "Track",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormattedGoogleLocation",
                table: "Track");

            migrationBuilder.DropColumn(
                name: "GooglePlacesId",
                table: "Track");

            migrationBuilder.DropColumn(
                name: "IsVerifiedByAdmin",
                table: "Track");
        }
    }
}
