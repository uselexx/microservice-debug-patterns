namespace Shared.Contracts;

public record MistralRequest(string Prompt);

public  record MistralResponse(string Message);