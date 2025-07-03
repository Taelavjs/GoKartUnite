using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class friendStatusNotificationTabl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendStatusNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: false),
                    FriendId = table.Column<int>(type: "int", nullable: false),
                    FriendName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendStatusNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendStatusNotifications_Karter_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendStatusNotifications_Karter_UserId",
                        column: x => x.UserId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendStatusNotifications_FriendId",
                table: "FriendStatusNotifications",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendStatusNotifications_UserId",
                table: "FriendStatusNotifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendStatusNotifications");
        }
    }
}
