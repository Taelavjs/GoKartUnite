using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class MembershipTableLinksKartersAndGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Karter_HostId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Karter_Groups_GroupId",
                table: "Karter");

            migrationBuilder.DropIndex(
                name: "IX_Karter_GroupId",
                table: "Karter");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Karter");

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    KarterId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    MemberRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => new { x.GroupId, x.KarterId });
                    table.ForeignKey(
                        name: "FK_Memberships_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Memberships_Karter_KarterId",
                        column: x => x.KarterId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_KarterId",
                table: "Memberships",
                column: "KarterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Karter_HostId",
                table: "Groups",
                column: "HostId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Karter_HostId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Karter",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Karter_GroupId",
                table: "Karter",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Karter_HostId",
                table: "Groups",
                column: "HostId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Karter_Groups_GroupId",
                table: "Karter",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
