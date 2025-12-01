using Shared.Abstractions.ValueObjects;

namespace Shared.Abstractions.Services;

public interface IRetroCreationService
{
    Task<Guid> CreateRetroFromTeamAsync(TeamId teamId, string title, List<UserId> participantIds, List<Email> participantEmails);
} 