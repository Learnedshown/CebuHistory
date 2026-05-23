using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CebuHistory.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_AspNetUsers_UserId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Stories_StoryId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Stories_StoryId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_UploadedById",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_UploadedById",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Stories_Era",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_PublishedAt",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Downloads",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Downloads",
                table: "Documents");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_AspNetUsers_UserId",
                table: "Bookmarks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Stories_StoryId",
                table: "Bookmarks",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Stories_StoryId",
                table: "Comments",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_UploadedById",
                table: "Documents",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_UploadedById",
                table: "Photos",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_AspNetUsers_UserId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Stories_StoryId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Stories_StoryId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_UploadedById",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_UploadedById",
                table: "Photos");

            migrationBuilder.AddColumn<int>(
                name: "Downloads",
                table: "Photos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Downloads",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Stories_Era",
                table: "Stories",
                column: "Era");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_PublishedAt",
                table: "Stories",
                column: "PublishedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_AspNetUsers_UserId",
                table: "Bookmarks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Stories_StoryId",
                table: "Bookmarks",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Stories_StoryId",
                table: "Comments",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_UploadedById",
                table: "Documents",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_UploadedById",
                table: "Photos",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
