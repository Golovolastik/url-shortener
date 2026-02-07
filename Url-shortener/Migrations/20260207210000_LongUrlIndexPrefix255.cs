using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Url_shortener.Migrations
{
    /// <inheritdoc />
    public partial class LongUrlIndexPrefix255 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Non-unique index on first 255 chars so lookups by LongUrl are faster; full column is longtext.
            migrationBuilder.Sql("CREATE INDEX IX_ShortenedUrls_LongUrl ON ShortenedUrls (LongUrl(255));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IX_ShortenedUrls_LongUrl ON ShortenedUrls;");
        }
    }
}
