using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Url_shortener.Migrations
{
    /// <inheritdoc />
    public partial class LongUrlLongText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_ShortenedUrls_LongUrl", table: "ShortenedUrls");
            migrationBuilder.Sql("ALTER TABLE ShortenedUrls MODIFY COLUMN LongUrl LONGTEXT NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE ShortenedUrls MODIFY COLUMN LongUrl VARCHAR(255) NOT NULL;");
            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_LongUrl",
                table: "ShortenedUrls",
                column: "LongUrl",
                unique: true);
        }
    }
}
