using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class karterTrackStatsDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KarterTrackStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RaceLength = table.Column<int>(type: "int", nullable: false),
                    isChampionshipRace = table.Column<bool>(type: "bit", nullable: false),
                    RecordedTrackId = table.Column<int>(type: "int", nullable: true),
                    ForKarterId = table.Column<int>(type: "int", nullable: true),
                    DateOnlyRecorded = table.Column<DateOnly>(type: "date", nullable: false),
                    WEATHERSTATUS = table.Column<int>(type: "int", nullable: false),
                    TEMPERATURE = table.Column<int>(type: "int", nullable: false),
                    BestLapTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KarterTrackStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KarterTrackStats_Karter_ForKarterId",
                        column: x => x.ForKarterId,
                        principalTable: "Karter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KarterTrackStats_Track_RecordedTrackId",
                        column: x => x.RecordedTrackId,
                        principalTable: "Track",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KarterTrackStats_ForKarterId",
                table: "KarterTrackStats",
                column: "ForKarterId");

            migrationBuilder.CreateIndex(
                name: "IX_KarterTrackStats_RecordedTrackId",
                table: "KarterTrackStats",
                column: "RecordedTrackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KarterTrackStats");
        }
    }
}
