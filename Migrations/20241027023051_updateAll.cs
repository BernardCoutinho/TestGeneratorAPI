using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGeneratorAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FolderName",
                table: "Folder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Folder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "FileContexts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "FileAnswers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "FileAnswers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Folder_UserId",
                table: "Folder",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Folder_Users_UserId",
                table: "Folder",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folder_Users_UserId",
                table: "Folder");

            migrationBuilder.DropIndex(
                name: "IX_Folder_UserId",
                table: "Folder");

            migrationBuilder.DropColumn(
                name: "FolderName",
                table: "Folder");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Folder");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "FileAnswers");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Content",
                table: "FileContexts",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Content",
                table: "FileAnswers",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
