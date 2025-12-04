using Teams.Core.Commands.Dtos;
using Teams.Core.Exceptions;
using Teams.Core.Queries.Dtos;
using Teams.Core.Repositories;
using Teams.Core.Events;
using Teams.Domain.Entities;
using Teams.Domain.Enums;
using Teams.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;
using Shared.Abstractions.Services;
using Shared.Abstractions.Events;
using Shared.Infrastructure.Helpers;
using Retro.Domain.ValueObjects;

namespace Teams.Core.Commands;

public class TeamCommandService : ITeamCommandService
{
    private readonly ITeamRepository _teamRepository;
    private readonly HttpContextHelper _httpContextHelper;
    private readonly IClock _clock;
    private readonly IEventPublisher _eventPublisher;

    public TeamCommandService(
        ITeamRepository teamRepository,
        HttpContextHelper httpContextHelper,
        IClock clock,
        IEventPublisher eventPublisher)
    {
        _teamRepository = teamRepository;
        _httpContextHelper = httpContextHelper;
        _clock = clock;
        _eventPublisher = eventPublisher;
    }

    public async Task<TeamDto> CreateTeamAsync(CreateTeamDto dto)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var teamId = new TeamId(Guid.NewGuid());

        var team = new Team.TeamBuilder()
            .WithId(teamId)
            .WithName(new TeamName(dto.Name))
            .WithDescription(new TeamDescription(dto.Description ?? string.Empty))
            .WithCreatedAt(new CreatedAtDate(_clock.UtcNow))
            .WithCreatedBy(user.Id)
            .Build();

        // Add creator as owner
        var ownerParticipant = new TeamParticipant
        {
            UserId = user.Id,
            TeamId = teamId,
            Email = user.Email,
            Role = TeamParticipantRole.Owner,
            Status = TeamParticipantStatus.Active,
            JoinedAt = new CreatedAtDate(_clock.UtcNow)
        };

        team.AddParticipant(ownerParticipant);
        await _teamRepository.SaveAsync(team);

        return TeamDto.FromDomain(team);
    }

    public async Task<TeamDto> UpdateTeamAsync(TeamId teamId, UpdateTeamDto dto)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team == null)
            throw new InvalidOperationException("Team not found.");

        if (!team.IsAdmin(user.Id))
            throw new UnauthorizedAccessException("Only team admins can update team details.");

        team.UpdateName(new TeamName(dto.Name));
        team.UpdateDescription(new TeamDescription(dto.Description ?? string.Empty));

        await _teamRepository.UpdateAsync(team);

        return TeamDto.FromDomain(team);
    }

    public async Task DeleteTeamAsync(TeamId teamId)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team == null)
            throw new InvalidOperationException("Team not found.");

        if (!team.IsOwner(user.Id))
            throw new UnauthorizedAccessException("Only team owners can delete the team.");

        await _teamRepository.DeleteAsync(teamId);
    }

    public async Task AddParticipantAsync(TeamId teamId, AddTeamParticipantDto dto)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team == null)
            throw new InvalidOperationException("Team not found.");

        if (!team.IsAdmin(user.Id))
            throw new UnauthorizedAccessException("Only team admins can add participants.");

        var participant = new TeamParticipant
        {
            UserId = new UserId(Guid.NewGuid()),
            TeamId = teamId,
            Email = new Email(dto.Email),
            Role = (TeamParticipantRole)dto.Role,
            Status = TeamParticipantStatus.Invited,
            JoinedAt = new CreatedAtDate(_clock.UtcNow)
        };

        try
        {
            team.AddParticipant(participant);

            await _teamRepository.AddParticipantAsync(teamId, participant);

            await _eventPublisher.PublishAsync(new TeamParticipantInvitedEvent(
                teamId,
                team.Name,
                user.Email,
                new Email(dto.Email)
            ));
        }
        catch (DomainModelArgumentException ex) when (ex.Message.Contains("email already exists"))
        {
            SentrySdk.CaptureException(ex);
            throw new TeamParticipantAlreadyExistsException(dto.Email);
        }
    }

    public async Task RemoveParticipantAsync(TeamId teamId, UserId userId)
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team == null)
            throw new InvalidOperationException("Team not found.");

        if (!team.IsAdmin(currentUser.Id) && currentUser.Id.Value != userId.Value)
            throw new UnauthorizedAccessException(
                "Only team admins or the participant themselves can remove participants.");

        team.RemoveParticipant(userId);
        await _teamRepository.RemoveParticipantAsync(teamId, userId);
    }

    public async Task AcceptInvitationAsync(TeamId teamId)
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team == null)
            throw new InvalidOperationException("Team not found.");

        // Find invited participant entry by email
        var participant = team.Participants
            .FirstOrDefault(p => p.Email.Value.Equals(currentUser.Email.Value, StringComparison.OrdinalIgnoreCase)
                                 && p.Status == TeamParticipantStatus.Invited);

        if (participant == null)
            throw new UnauthorizedAccessException("No pending invitation for current user.");

        // Assign real user id and activate
        participant.UserId = currentUser.Id;
        participant.Activate();

        await _teamRepository.AcceptInvitationAsync(teamId, currentUser.Id, currentUser.Email);
    }

    public async Task DenyInvitationAsync(TeamId teamId)
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team == null)
            throw new InvalidOperationException("Team not found.");

        var hasPending = team.Participants.Any(p =>
            p.Email.Value.Equals(currentUser.Email.Value, StringComparison.OrdinalIgnoreCase) &&
            p.Status == TeamParticipantStatus.Invited);

        if (!hasPending)
            throw new UnauthorizedAccessException("No pending invitation for current user.");

        await _teamRepository.DenyInvitationAsync(teamId, currentUser.Email);
    }
}