using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CebuHistory.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSubmissionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Stories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Stories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedByName",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedByUserId",
                table: "Stories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stories_SubmittedByUserId",
                table: "Stories",
                column: "SubmittedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_AspNetUsers_SubmittedByUserId",
                table: "Stories",
                column: "SubmittedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_AspNetUsers_SubmittedByUserId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_SubmittedByUserId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "SubmittedByName",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "SubmittedByUserId",
                table: "Stories");
        }
    }
}
