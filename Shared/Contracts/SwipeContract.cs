namespace Shared.Contracts;

public record GetSwipesRequest(string UserId);
public record GetSwipesResponse(List<SwipeDto> Swipes);
public record PostSwipeRequest(string UserId, SwipeDto Swipe);
public record PostSwipeResponse(bool Success, string? Error);

public record SwipeDto(
    Guid Id,
    int MovieId,
    bool IsLiked,
    DateTime Timestamp);