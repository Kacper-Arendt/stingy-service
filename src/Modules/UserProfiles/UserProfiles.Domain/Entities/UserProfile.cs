using UserProfiles.Domain.Enums;
using UserProfiles.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Domain.Entities;

public class UserProfile
{
    public UserId UserId { get; private set; } = null!;
    public DisplayName DisplayName { get; private set; } = null!;
    public ProfileImageUrl? ProfileImageUrl { get; private set; }
    public UserBio? Bio { get; private set; }
    public DateTimeZone TimeZone { get; private set; } = null!;
    public ProfileVisibilityLevel Visibility { get; private set; }

    public CreatedAtDate CreatedAt { get; private set; } = null!;
    public LastModifiedDate LastModifiedAt { get; private set; } = null!;

    private UserProfile()
    {
        // EF Core constructor
    }

    #region Builder

    public class UserProfileBuilder
    {
        private UserId _userId = null!;
        private DisplayName _displayName = null!;
        private ProfileImageUrl? _profileImageUrl;
        private UserBio? _bio;
        private DateTimeZone _timeZone = new(); // Default: Europe/Warsaw
        private ProfileVisibilityLevel _visibility = ProfileVisibilityLevel.Public;

        private CreatedAtDate _createdAt = new CreatedAtDate(DateTime.UtcNow);
        private LastModifiedDate _lastModifiedAt = new LastModifiedDate(DateTime.UtcNow);

        public UserProfileBuilder WithUserId(UserId userId)
        {
            _userId = userId ?? throw new DomainModelArgumentException("UserId cannot be null.");
            return this;
        }

        public UserProfileBuilder WithDisplayName(DisplayName displayName)
        {
            _displayName = displayName ?? throw new DomainModelArgumentException("DisplayName cannot be null.");
            return this;
        }

        public UserProfileBuilder WithProfileImageUrl(ProfileImageUrl? profileImageUrl)
        {
            _profileImageUrl = profileImageUrl;
            return this;
        }

        public UserProfileBuilder WithBio(UserBio? bio)
        {
            _bio = bio;
            return this;
        }

        public UserProfileBuilder WithTimeZone(DateTimeZone timeZone)
        {
            _timeZone = timeZone ?? throw new DomainModelArgumentException("TimeZone cannot be null.");
            return this;
        }

        public UserProfileBuilder WithVisibility(ProfileVisibilityLevel visibility)
        {
            _visibility = visibility;
            return this;
        }



        public UserProfileBuilder WithCreatedAt(CreatedAtDate createdAt)
        {
            _createdAt = createdAt ?? throw new DomainModelArgumentException("CreatedAt cannot be null.");
            return this;
        }

        public UserProfileBuilder WithLastModifiedAt(LastModifiedDate lastModifiedAt)
        {
            _lastModifiedAt = lastModifiedAt ?? throw new DomainModelArgumentException("LastModifiedAt cannot be null.");
            return this;
        }

        public UserProfile Build()
        {
            ValidateBuilder();

            return new UserProfile
            {
                UserId = _userId,
                DisplayName = _displayName,
                ProfileImageUrl = _profileImageUrl,
                Bio = _bio,
                TimeZone = _timeZone,
                Visibility = _visibility,

                CreatedAt = _createdAt,
                LastModifiedAt = _lastModifiedAt
            };
        }

        private void ValidateBuilder()
        {
            if (_userId == null)
                throw new DomainModelArgumentException("UserId is required.");

            if (_displayName == null)
                throw new DomainModelArgumentException("DisplayName is required.");

            if (_timeZone == null)
                throw new DomainModelArgumentException("TimeZone is required.");

            if (_createdAt == null)
                throw new DomainModelArgumentException("CreatedAt is required.");

            if (_lastModifiedAt == null)
                throw new DomainModelArgumentException("LastModifiedAt is required.");

            if (_lastModifiedAt.Value < _createdAt.Value)
                throw new DomainModelArgumentException("LastModifiedAt cannot be earlier than CreatedAt.");
        }
    }

    #endregion

    #region Business Methods

    public void UpdateProfile(DisplayName displayName, ProfileImageUrl? profileImageUrl, UserBio? bio, DateTimeZone timeZone, ProfileVisibilityLevel visibility)
    {
        DisplayName = displayName ?? throw new DomainModelArgumentException("DisplayName cannot be null.");
        ProfileImageUrl = profileImageUrl;
        Bio = bio;
        TimeZone = timeZone ?? throw new DomainModelArgumentException("TimeZone cannot be null.");
        Visibility = visibility;
        LastModifiedAt = new LastModifiedDate(DateTime.UtcNow);
    }

    public bool IsVisibleForUser(UserId? viewerId)
    {
        return Visibility switch
        {
            ProfileVisibilityLevel.Public => true,
            ProfileVisibilityLevel.Limited => true,
            ProfileVisibilityLevel.Private => viewerId != null && UserId.Value == viewerId.Value,
            _ => false
        };
    }

    public bool CanViewFullProfile(UserId? viewerId)
    {
        return Visibility switch
        {
            ProfileVisibilityLevel.Public => true,
            ProfileVisibilityLevel.Limited => viewerId != null && UserId.Value == viewerId.Value,
            ProfileVisibilityLevel.Private => viewerId != null && UserId.Value == viewerId.Value,
            _ => false
        };
    }



    #endregion

    #region Factory Methods

    public static UserProfile CreateDefault(UserId userId, DisplayName displayName)
    {
        return new UserProfileBuilder()
            .WithUserId(userId)
            .WithDisplayName(displayName)
            .WithTimeZone(new DateTimeZone()) // Europe/Warsaw
            .WithVisibility(ProfileVisibilityLevel.Public)
            .Build();
    }

    public static UserProfile CreateFromEmail(UserId userId, Email email)
    {
        var emailParts = email.Value.Split('@');
        var displayName = new DisplayName(emailParts[0]);
        
        return CreateDefault(userId, displayName);
    }

    #endregion
}
