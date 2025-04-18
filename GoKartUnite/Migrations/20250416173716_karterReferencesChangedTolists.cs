using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class karterReferencesChangedTolists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowTracks_Karter_KarterId",
                table: "FollowTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowTracks_Track_TrackId",
                table: "FollowTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Karter_Track_TrackId",
                table: "Karter");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowTracks_Karter_KarterId",
                table: "FollowTracks",
                column: "KarterId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FollowTracks_Track_TrackId",
                table: "FollowTracks",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Karter_Track_TrackId",
                table: "Karter",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowTracks_Karter_KarterId",
                table: "FollowTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowTracks_Track_TrackId",
                table: "FollowTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Karter_Track_TrackId",
                table: "Karter");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowTracks_Karter_KarterId",
                table: "FollowTracks",
                column: "KarterId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FollowTracks_Track_TrackId",
                table: "FollowTracks",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Karter_Track_TrackId",
                table: "Karter",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id");
        }
    }
}
