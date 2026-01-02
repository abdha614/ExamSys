using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class addlecturefile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LectureFiles_FileHash",
                table: "LectureFiles");

            migrationBuilder.DropColumn(
                name: "FileHash",
                table: "LectureFiles");

            migrationBuilder.DropColumn(
                name: "VectorStoreId",
                table: "LectureFiles");

            migrationBuilder.RenameColumn(
                name: "RemoteFileId",
                table: "LectureFiles",
                newName: "StoredFileName");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "LectureFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEN260ocARayzDoOjlds9NhyUB2QxnpLG49gRbxznktI1rQJTdu3SOxCz1dohTX3Y0g==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "LectureFiles");

            migrationBuilder.RenameColumn(
                name: "StoredFileName",
                table: "LectureFiles",
                newName: "RemoteFileId");

            migrationBuilder.AddColumn<string>(
                name: "FileHash",
                table: "LectureFiles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VectorStoreId",
                table: "LectureFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMugNparlaKJ7aEtTntM8+ZqTOjX6YtcFc3+4ySMEgEVGOyYHsEkRpv3mRswlPQhxg==");

            migrationBuilder.CreateIndex(
                name: "IX_LectureFiles_FileHash",
                table: "LectureFiles",
                column: "FileHash",
                unique: true);
        }
    }
}
