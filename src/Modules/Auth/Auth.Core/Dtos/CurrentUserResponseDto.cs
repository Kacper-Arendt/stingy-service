namespace Auth.Core.Dtos;

public record CurrentUserResponseDto(string Name, string Id, IEnumerable<string> Roles);
