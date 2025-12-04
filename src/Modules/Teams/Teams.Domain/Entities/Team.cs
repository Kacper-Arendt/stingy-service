using Teams.Domain.Enums;
using Teams.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;
using Retro.Domain.ValueObjects;

namespace Teams.Domain.Entities;

public class Team
{
    public TeamId Id { get; private set; } = null!;
    public TeamName Name { get; private set; } = null!;
    public TeamDescription Description { get; private set; } = null!;
    public CreatedAtDate CreatedAt { get; private set; } = null!;
    public UserId CreatedBy { get; private set; } = null!;
    public List<TeamParticipant> Participants { get; private set; }

    private Team()
    {
        Participants = new List<TeamParticipant>();
    }

    #region Builder

    public class TeamBuilder
    {
        private TeamId _id = null!;
        private TeamName _name = null!;
        private TeamDescription _description = new TeamDescription(string.Empty);
        private CreatedAtDate _createdAt = new CreatedAtDate(DateTime.UtcNow);
        private UserId _createdBy = null!;
        private List<TeamParticipant> _participants = new List<TeamParticipant>();

        public TeamBuilder WithId(TeamId id)
        {
            _id = id;
            return this;
        }

        public TeamBuilder WithName(TeamName name)
        {
            _name = name;
            return this;
        }

        public TeamBuilder WithDescription(TeamDescription description)
        {
            _description = description;
            return this;
        }

        public TeamBuilder WithCreatedAt(CreatedAtDate createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public TeamBuilder WithCreatedBy(UserId createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public TeamBuilder WithParticipants(List<TeamParticipant> participants)
        {
            _participants = participants ?? new List<TeamParticipant>();
            return this;
        }

        public Team Build()
        {
            if (_id == null) throw new DomainModelArgumentException("TeamId is required.");
            if (_name == null) throw new DomainModelArgumentException("TeamName is required.");
            if (_createdBy == null) throw new DomainModelArgumentException("CreatedBy is required.");

            return new Team
            {
                Id = _id,
                Name = _name,
                Description = _description,
                CreatedAt = _createdAt,
                CreatedBy = _createdBy,
                Participants = _participants
            };
        }
    }

    #endregion

    public void AddParticipant(TeamParticipant participant)
    {
        if (participant == null)
            throw new DomainModelArgumentException("Participant cannot be null.");

        if (Participants.Any(p => p.UserId.Value == participant.UserId.Value))
            throw new DomainModelArgumentException("Participant with the same UserId already exists.");

        if (Participants.Any(p => p.Email.Value == participant.Email.Value))
            throw new DomainModelArgumentException("Participant with the same email already exists.");

        Participants.Add(participant);
    }

    public void RemoveParticipant(UserId userId)
    {
        var participant = Participants.FirstOrDefault(p => p.UserId.Value == userId.Value);
        if (participant == null)
            throw new DomainModelArgumentException($"Participant with UserId {userId.Value} not found.");

        if (participant.Role == TeamParticipantRole.Owner && 
            Participants.Count(p => p.Role == TeamParticipantRole.Owner) <= 1)
            throw new DomainModelArgumentException("Cannot remove the last owner participant.");

        Participants.Remove(participant);
    }

    public TeamParticipant GetParticipant(UserId userId)
        => Participants.FirstOrDefault(p => p.UserId.Value == userId.Value)
           ?? throw new DomainModelArgumentException($"Participant with UserId {userId.Value} not found.");

    public bool IsParticipant(UserId userId)
        => Participants.Any(p => p.UserId.Value == userId.Value);

    public bool IsOwner(UserId userId)
        => Participants.Any(p => p.UserId.Value == userId.Value && p.Role == TeamParticipantRole.Owner);

    public bool IsAdmin(UserId userId)
        => Participants.Any(p => p.UserId.Value == userId.Value && 
                                 (p.Role == TeamParticipantRole.Owner || p.Role == TeamParticipantRole.Admin));

    public void UpdateName(TeamName newName)
    {
        if (newName == null)
            throw new DomainModelArgumentException("TeamName cannot be null.");

        Name = newName;
    }

    public void UpdateDescription(TeamDescription newDescription)
    {
        Description = newDescription ?? new TeamDescription(string.Empty);
    }

    public List<TeamParticipant> GetActiveParticipants()
        => Participants.Where(p => p.Status == TeamParticipantStatus.Active).ToList();
} 