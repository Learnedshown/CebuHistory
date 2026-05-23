using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CebuHistory.Migrations
{
    /// <inheritdoc />
    public partial class FixSubmittedByUserCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_UploadedById",
                table: "Documents");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_UploadedById",
                table: "Documents",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_UploadedById",
                table: "Documents");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_UploadedById",
                table: "Documents",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
