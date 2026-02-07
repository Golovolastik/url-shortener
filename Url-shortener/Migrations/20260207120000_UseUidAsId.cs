using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Url_shortener.Migrations
{
    /// <inheritdoc />
    public partial class UseUidAsId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Id is set from uid (ulong) in code, so we drop auto-increment and use BIGINT UNSIGNED to match ulong.
            migrationBuilder.Sql("ALTER TABLE ShortenedUrls MODIFY COLUMN Id BIGINT UNSIGNED NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE ShortenedUrls MODIFY COLUMN Id INT NOT NULL AUTO_INCREMENT;");
        }
    }
}
