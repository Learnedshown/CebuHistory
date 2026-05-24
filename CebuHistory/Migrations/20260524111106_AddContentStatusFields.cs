using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CebuHistory.Migrations
{
    /// <inheritdoc />
    public partial class AddContentStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Photos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Photos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedByName",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedByUserId",
                table: "Photos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedByName",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedByUserId",
                table: "Documents",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_SubmittedByUserId",
                table: "Photos",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SubmittedByUserId",
                table: "Documents",
                column: "SubmittedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_SubmittedByUserId",
                table: "Documents",
                column: "SubmittedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_SubmittedByUserId",
                table: "Photos",
                column: "SubmittedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_SubmittedByUserId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_SubmittedByUserId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_SubmittedByUserId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Documents_SubmittedByUserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "SubmittedByName",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "SubmittedByUserId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SubmittedByName",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SubmittedByUserId",
                table: "Documents");
        }
    }
}
