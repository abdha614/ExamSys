using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddcodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailConfirmationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailConfirmationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailConfirmationCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFQtE9EjcHT1XJUaXH6xqqtgvRGlpPdK/TGstGy67uuhDv9qyKisodrmVXJCgjnzaA==");

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmationCodes_Code",
                table: "EmailConfirmationCodes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmationCodes_Email",
                table: "EmailConfirmationCodes",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmationCodes_UserId",
                table: "EmailConfirmationCodes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailConfirmationCodes");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKesBwqfspFBobyHTP1BlnkgKZwgwj32ENF7a57ZbO6A9wHLIZl8lkriwAWqnGNG/w==");
        }
    }
}
