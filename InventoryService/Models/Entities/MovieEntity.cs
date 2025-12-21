namespace InventoryService.Models.Entities;

public class MovieEntity
{

    /// <summary>
    /// ID for movie in new system
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Old Identifier from dataset where movie was exported from
    /// </summary>
    public int LegacyId { get; set; }

    public string? Title { get; set; }

    public decimal VoteAverage { get; set; }
    public MovieStatus Status { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public long Revenue { get; set; }

    public int Runtime { get; set; }

    public bool AdultsOnly { get; set; }

    public long Budget { get; set; }

    public string? Homepage { get; set; }

    public string? ImdbId { get; set; }

    public string? OriginalLanguage { get; set; }

    public string? OriginalTitle { get; set; }

    /// <summary>
    /// Short description of the movie
    /// </summary>
    public string? Overview { get; set; }

    public double Popularity { get; set; }

    /// <summary>
    /// Catchphrase or memorable line associated with the movie
    /// </summary>
    public string? Tagline { get; set; }

    public string? Genres { get; set; }

    public string? ProductionCompanies { get; set; }

    public string? ProductionCountries { get; set; }

    public string? SpokenLanguages { get; set; }

    public string? Keywords { get; set; }
}

public enum MovieStatus
{
    Released,
    Unknown
}
