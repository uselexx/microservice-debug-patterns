using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LegacyId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    VoteAverage = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "date", nullable: true),
                    Revenue = table.Column<long>(type: "bigint", nullable: false),
                    Runtime = table.Column<int>(type: "integer", nullable: false),
                    AdultsOnly = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Budget = table.Column<long>(type: "bigint", nullable: false),
                    Homepage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImdbId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OriginalLanguage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    OriginalTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Overview = table.Column<string>(type: "text", nullable: true),
                    Popularity = table.Column<double>(type: "double precision", precision: 10, scale: 4, nullable: false),
                    Tagline = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Genres = table.Column<string>(type: "text", nullable: true),
                    ProductionCompanies = table.Column<string>(type: "text", nullable: true),
                    ProductionCountries = table.Column<string>(type: "text", nullable: true),
                    SpokenLanguages = table.Column<string>(type: "text", nullable: true),
                    Keywords = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movies_ImdbId",
                table: "movies",
                column: "ImdbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movies_LegacyId",
                table: "movies",
                column: "LegacyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movies_ReleaseDate",
                table: "movies",
                column: "ReleaseDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movies");
        }
    }
}
