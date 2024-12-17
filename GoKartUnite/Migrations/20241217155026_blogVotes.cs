using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class blogVotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Upvotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoterId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    KarterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Upvotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Upvotes_BlogPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Upvotes_Karter_KarterId",
                        column: x => x.KarterId,
                        principalTable: "Karter",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Upvotes_KarterId",
                table: "Upvotes",
                column: "KarterId");

            migrationBuilder.CreateIndex(
                name: "IX_Upvotes_PostId",
                table: "Upvotes",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Upvotes");
        }
    }
}
