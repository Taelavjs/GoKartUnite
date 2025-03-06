using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class karterTrackStatsDbForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KarterTrackStats_Karter_ForKarterId",
                table: "KarterTrackStats");

            migrationBuilder.DropForeignKey(
                name: "FK_KarterTrackStats_Track_RecordedTrackId",
                table: "KarterTrackStats");

            migrationBuilder.DropIndex(
                name: "IX_KarterTrackStats_ForKarterId",
                table: "KarterTrackStats");

            migrationBuilder.DropIndex(
                name: "IX_KarterTrackStats_RecordedTrackId",
                table: "KarterTrackStats");

            migrationBuilder.DropColumn(
                name: "ForKarterId",
                table: "KarterTrackStats");

            migrationBuilder.DropColumn(
                name: "RecordedTrackId",
                table: "KarterTrackStats");

            migrationBuilder.AddColumn<int>(
                name: "KarterId",
                table: "KarterTrackStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "KarterTrackStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_KarterTrackStats_KarterId",
                table: "KarterTrackStats",
                column: "KarterId");

            migrationBuilder.CreateIndex(
                name: "IX_KarterTrackStats_TrackId",
                table: "KarterTrackStats",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_KarterTrackStats_Karter_KarterId",
                table: "KarterTrackStats",
                column: "KarterId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KarterTrackStats_Track_TrackId",
                table: "KarterTrackStats",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KarterTrackStats_Karter_KarterId",
                table: "KarterTrackStats");

            migrationBuilder.DropForeignKey(
                name: "FK_KarterTrackStats_Track_TrackId",
                table: "KarterTrackStats");

            migrationBuilder.DropIndex(
                name: "IX_KarterTrackStats_KarterId",
                table: "KarterTrackStats");

            migrationBuilder.DropIndex(
                name: "IX_KarterTrackStats_TrackId",
                table: "KarterTrackStats");

            migrationBuilder.DropColumn(
                name: "KarterId",
                table: "KarterTrackStats");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "KarterTrackStats");

            migrationBuilder.AddColumn<int>(
                name: "ForKarterId",
                table: "KarterTrackStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordedTrackId",
                table: "KarterTrackStats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KarterTrackStats_ForKarterId",
                table: "KarterTrackStats",
                column: "ForKarterId");

            migrationBuilder.CreateIndex(
                name: "IX_KarterTrackStats_RecordedTrackId",
                table: "KarterTrackStats",
                column: "RecordedTrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_KarterTrackStats_Karter_ForKarterId",
                table: "KarterTrackStats",
                column: "ForKarterId",
                principalTable: "Karter",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KarterTrackStats_Track_RecordedTrackId",
                table: "KarterTrackStats",
                column: "RecordedTrackId",
                principalTable: "Track",
                principalColumn: "Id");
        }
    }
}
