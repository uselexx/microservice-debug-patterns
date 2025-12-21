using CsvHelper.Configuration.Attributes;

namespace InventoryService.Models.Csv;

public class MovieCsvRow
{
    [Name("id")]
    public int LegacyId { get; set; }

    [Name("title")]
    public string? Title { get; set; }

    [Name("vote_average")]
    public decimal VoteAverage { get; set; }

    [Name("status")]
    public string? Status { get; set; }

    [Name("release_date")]
    public DateTime? ReleaseDate { get; set; }

    [Name("revenue")]
    public long Revenue { get; set; }

    [Name("runtime")]
    public int Runtime { get; set; }

    [Name("adult")]
    public bool AdultsOnly { get; set; }

    [Name("budget")]
    public long Budget { get; set; }

    [Name("homepage")]
    public string? Homepage { get; set; }

    [Name("imdb_id")]
    public string? ImdbId { get; set; }

    [Name("original_language")]
    public string? OriginalLanguage { get; set; }

    [Name("original_title")]
    public string? OriginalTitle { get; set; }


    [Name("overview")]
    public string? Overview { get; set; }

    [Name("popularity")]
    public double Popularity { get; set; }

    [Name("tagline")]
    public string? Tagline { get; set; }

    [Name("genres")]
    public string? Genres { get; set; }

    [Name("production_companies")]
    public string? ProductionCompanies { get; set; }

    [Name("production_countries")]
    public string? ProductionCountries { get; set; }

    [Name("spoken_languages")]
    public string? SpokenLanguages { get; set; }

    [Name("keywords")]
    public string? Keywords { get; set; }
}
