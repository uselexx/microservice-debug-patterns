using CsvHelper;
using CsvHelper.Configuration;
using InventoryService.Models.Csv;
using InventoryService.Models.Entities;
using InventoryService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using System.Diagnostics;
using System.Globalization;

namespace InventoryService.Jobs;

[DisallowConcurrentExecution]
public class MovieImportJob(MovieImportService service, ILogger<MovieImportJob> logger) : IJob
{
    private const int BatchSize = 1_000;
    private const int LogEvery = 5_000; // reduce noise
    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("🎬 [MovieImport] Job started");

        var csvPath = "./Data/movies.csv";
        if (!File.Exists(csvPath))
        {
            logger.LogWarning("⚠️ [MovieImport] CSV file not found at {Path}", csvPath);
            return;
        }

        //var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        //{
        //    HasHeaderRecord = true,
        //    MissingFieldFound = null,
        //    BadDataFound = null,
        //    HeaderValidated = null
        //};

        //// ✅ Validate CSV first (fail fast)
        //var valid = await ValidateLegacyIdsAreUniqueAsync(
        //    csvPath,
        //    csvConfig,
        //    logger,
        //    cancellationToken);

        //if (!valid)
        //{
        //    logger.LogError("❌ [MovieImport] Job aborted due to invalid CSV");
        //    return;
        //}

        using var stream = File.OpenRead(csvPath);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var seenLegacyIds = new HashSet<int>();
        var batch = new List<MovieEntity>(BatchSize);
        var processed = 0;
        var skippedDuplicates = 0;
        var read = 0;
        await foreach (var row in csv.GetRecordsAsync<MovieCsvRow>(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            read++;

            // Deduplication (first row wins)
            if (!seenLegacyIds.Add(row.LegacyId))
            {
                skippedDuplicates++;

                if (skippedDuplicates <= 10)
                {
                    logger.LogWarning(
                        "⚠️ [MovieImport] Duplicate LegacyId {LegacyId} skipped",
                        row.LegacyId);
                }

                continue;
            }

            batch.Add(MapToNewEntity(row));

            if (batch.Count >= BatchSize)
            {
                await service.ImportBatchAsync(batch, cancellationToken);
                processed += batch.Count;
                batch.Clear();

                LogProgress(processed, read, skippedDuplicates, stopwatch);
            }
        }

        if (batch.Count > 0)
        {
            await service.ImportBatchAsync(batch, cancellationToken);
            processed += batch.Count;
            batch.Clear();
        }

        stopwatch.Stop();

        logger.LogInformation(
            "✅ [MovieImport] Completed | 📥 Read {Read:N0} | 🎞 Imported {Imported:N0} | 🧹 Duplicates {Duplicates:N0} | ⏱ {Elapsed}",
            read,
            processed,
            skippedDuplicates,
            stopwatch.Elapsed);
    }
    private void LogProgress(
            int processed,
            int read,
            int skipped,
            Stopwatch stopwatch)
    {
        if (processed % LogEvery != 0)
            return;

        var elapsedSeconds = Math.Max(1, stopwatch.Elapsed.TotalSeconds);
        var rate = processed / elapsedSeconds;

        logger.LogInformation(
            "🎬 [MovieImport] ✔ {Imported:N0} imported | 📥 {Read:N0} read | 🧹 {Skipped:N0} dupes | ⏱ {Elapsed:mm\\:ss} | 🚀 {Rate:N0}/sec",
            processed,
            read,
            skipped,
            stopwatch.Elapsed,
            rate);
    }
    private static MovieEntity MapToNewEntity(MovieCsvRow row)
    {
        return new MovieEntity
        {
            LegacyId = row.LegacyId,
            Title = row.Title,
            VoteAverage = row.VoteAverage,
            Status = ParseStatus(row.Status),
            ReleaseDate = row.ReleaseDate,
            Revenue = row.Revenue,
            Runtime = row.Runtime,
            AdultsOnly = row.AdultsOnly,
            Budget = row.Budget,
            Homepage = row.Homepage,
            ImdbId = row.ImdbId,
            OriginalLanguage = row.OriginalLanguage,
            OriginalTitle = row.OriginalTitle,
            Overview = row.Overview,
            Popularity = row.Popularity,
            Tagline = row.Tagline,
            Genres = row.Genres,
            ProductionCompanies = row.ProductionCompanies,
            ProductionCountries = row.ProductionCountries,
            SpokenLanguages = row.SpokenLanguages,
            Keywords = row.Keywords
        };
    }

    private static MovieStatus ParseStatus(string? status)
        => Enum.TryParse<MovieStatus>(status, ignoreCase: true, out var parsed)
            ? parsed
            : MovieStatus.Unknown;

    private static async Task<bool> ValidateLegacyIdsAreUniqueAsync(
    string csvPath,
    CsvConfiguration csvConfig,
    ILogger logger,
    CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var seen = new HashSet<int>();
        var duplicates = new HashSet<int>();
        var processed = 0;

        using var stream = File.OpenRead(csvPath);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, csvConfig);

        await foreach (var row in csv.GetRecordsAsync<MovieCsvRow>(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            processed++;
            if (!seen.Add(row.LegacyId))
            {
                duplicates.Add(row.LegacyId);
            }
            if (processed % LogEvery == 0)
            {
                logger.LogInformation(
                    "🔎 [MovieImport] Validation progress | {Count:N0} rows | ⏱ {Elapsed:mm\\:ss}",
                    processed,
                    stopwatch.Elapsed);
            }
        }

        stopwatch.Stop();

        if (duplicates.Count > 0)
        {
            logger.LogError(
                "❌ [MovieImport] CSV contains {Count} duplicate LegacyIds. Example(s): {Examples}",
                duplicates.Count,
                string.Join(", ", duplicates.Take(10)));

            return false;
        }

        logger.LogInformation(
            "✅ [MovieImport] CSV validation passed | {Count:N0} rows | ⏱ {Elapsed:mm\\:ss}",
            processed,
            stopwatch.Elapsed);

        return true;
    }
}
