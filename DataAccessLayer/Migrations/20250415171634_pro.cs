using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class pro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfessorId",
                table: "lectures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFpqWPM7/rceGePZaf9NhU4pxaqy4ZLRTw4QscKHoK/yzgo125ZBK+S95TSjM/mCww==");

            migrationBuilder.CreateIndex(
                name: "IX_lectures_ProfessorId",
                table: "lectures",
                column: "ProfessorId");

            migrationBuilder.AddForeignKey(
                name: "FK_lectures_Users_ProfessorId",
                table: "lectures",
                column: "ProfessorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lectures_Users_ProfessorId",
                table: "lectures");

            migrationBuilder.DropIndex(
                name: "IX_lectures_ProfessorId",
                table: "lectures");

            migrationBuilder.DropColumn(
                name: "ProfessorId",
                table: "lectures");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPuN5K1N3nxXImgIWZDnX3er6WV9FkRBdEGNAIxItfJEPMzWpjGta7DghiceTDjArg==");
        }
    }
}
