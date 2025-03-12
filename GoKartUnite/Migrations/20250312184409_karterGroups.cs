using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class karterGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Karter",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "BlogPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Karter_HostId",
                        column: x => x.HostId,
                        principalTable: "Karter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Karter_GroupId",
                table: "Karter",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_GroupId",
                table: "BlogPosts",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_HostId",
                table: "Groups",
                column: "HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Groups_GroupId",
                table: "BlogPosts",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Karter_Groups_GroupId",
                table: "Karter",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Groups_GroupId",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Karter_Groups_GroupId",
                table: "Karter");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Karter_GroupId",
                table: "Karter");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_GroupId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Karter");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "BlogPosts");
        }
    }
}
