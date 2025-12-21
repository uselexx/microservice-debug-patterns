namespace Shared.Contracts;

// Requests
public record GetMovieRequest(
    string? Title,
    string? Description,
    int? LegacyId);

public record GetMoviesRequest(int? cursor, int pageSize);

// Response

public record GetMoviesResponse(PagedResponse<GetMovieResponse> Movies);
public record GetMovieResponse(
    Guid? CorrelationId,
    MovieDto? Movie,
    string? Error);

public record MovieDto(
    int Id,
    int LegacyId,
    string Title,
    string? Overview,
    DateTime ReleaseDate,
    double Popularity);

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int? NextCursor { get; set; }
    public bool HasMore { get; set; }

    public PagedResponse(List<T> data, int? nextCursor, bool hasMore)
    {
        Data = data;
        NextCursor = nextCursor;
        HasMore = hasMore;
    }
}
