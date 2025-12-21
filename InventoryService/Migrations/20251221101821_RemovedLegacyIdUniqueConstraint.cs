using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class RemovedLegacyIdUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_movies_LegacyId",
                table: "movies");

            migrationBuilder.CreateIndex(
                name: "IX_movies_LegacyId",
                table: "movies",
                column: "LegacyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_movies_LegacyId",
                table: "movies");

            migrationBuilder.CreateIndex(
                name: "IX_movies_LegacyId",
                table: "movies",
                column: "LegacyId",
                unique: true);
        }
    }
}
