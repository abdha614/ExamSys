using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdFromEmailConfirmationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailConfirmationCodes_Users_UserId",
                table: "EmailConfirmationCodes");

            migrationBuilder.DropIndex(
                name: "IX_EmailConfirmationCodes_UserId",
                table: "EmailConfirmationCodes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmailConfirmationCodes");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELDJFWfOEBf0Uhq7L3E2f3Ff0/Qg+GYmAoetIYGSorqnkWYMqAm5urX9CP9LdbMkjg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "EmailConfirmationCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFQtE9EjcHT1XJUaXH6xqqtgvRGlpPdK/TGstGy67uuhDv9qyKisodrmVXJCgjnzaA==");

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmationCodes_UserId",
                table: "EmailConfirmationCodes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailConfirmationCodes_Users_UserId",
                table: "EmailConfirmationCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
