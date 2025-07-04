﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class exam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoGenerated",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Exams");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Exams",
                newName: "Metadata");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Exams",
                newName: "TotalQuestions");

            migrationBuilder.AlterColumn<double>(
                name: "Points",
                table: "ExamQuestions",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELHwM59ak2PqYX6QWkbJBiWTWGnA7fr4agH0bzcGq7UV74nJTbaxYotXggSpwB7gQA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalQuestions",
                table: "Exams",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "Metadata",
                table: "Exams",
                newName: "Note");

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoGenerated",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Exams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "ExamQuestions",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKLBHDr1AnGPebJ6VntAAuYFm5mzJwxzjQ4S0H1/hzagEMKjKPw0YOI5eT+9YQ6kvw==");
        }
    }
}
