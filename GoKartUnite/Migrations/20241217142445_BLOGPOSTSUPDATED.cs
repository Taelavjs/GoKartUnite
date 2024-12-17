using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoKartUnite.Migrations
{
    /// <inheritdoc />
    public partial class BLOGPOSTSUPDATED : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPost_Karter_AuthorId",
                table: "BlogPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPost",
                table: "BlogPost");

            migrationBuilder.RenameTable(
                name: "BlogPost",
                newName: "BlogPosts");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPost_AuthorId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_AuthorId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Karter",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Karter",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Karter_AuthorId",
                table: "BlogPosts",
                column: "AuthorId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Karter_AuthorId",
                table: "BlogPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts");

            migrationBuilder.RenameTable(
                name: "BlogPosts",
                newName: "BlogPost");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_AuthorId",
                table: "BlogPost",
                newName: "IX_BlogPost_AuthorId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Karter",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Karter",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPost",
                table: "BlogPost",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPost_Karter_AuthorId",
                table: "BlogPost",
                column: "AuthorId",
                principalTable: "Karter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
