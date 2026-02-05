using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Url_shortener.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesOnShortUrlAndLongUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortUrl",
                table: "ShortenedUrls",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "LongUrl",
                table: "ShortenedUrls",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_LongUrl",
                table: "ShortenedUrls",
                column: "LongUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_ShortUrl",
                table: "ShortenedUrls",
                column: "ShortUrl",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_LongUrl",
                table: "ShortenedUrls");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_ShortUrl",
                table: "ShortenedUrls");

            migrationBuilder.AlterColumn<string>(
                name: "ShortUrl",
                table: "ShortenedUrls",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "LongUrl",
                table: "ShortenedUrls",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");
        }
    }
}
