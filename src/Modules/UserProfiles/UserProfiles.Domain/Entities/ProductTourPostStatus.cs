using UserProfiles.Domain.Enums;
using UserProfiles.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Domain.Entities;

public class ProductTourPostStatus
{
    public Guid Id { get; private set; }
    public UserId UserId { get; private set; } = null!;
    public TourKey TourKey { get; private set; } = null!;
    public TourStatus Status { get; private set; }

    private ProductTourPostStatus()
    {
        // EF Core/Dapper constructor
    }

    #region Builder

    public class ProductTourPostStatusBuilder
    {
        private Guid _id = Guid.NewGuid();
        private UserId _userId = null!;
        private TourKey _tourKey = null!;
        private TourStatus _status;

        public ProductTourPostStatusBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public ProductTourPostStatusBuilder WithUserId(UserId userId)
        {
            _userId = userId ?? throw new DomainModelArgumentException("UserId cannot be null.");
            return this;
        }

        public ProductTourPostStatusBuilder WithTourKey(TourKey tourKey)
        {
            _tourKey = tourKey ?? throw new DomainModelArgumentException("TourKey cannot be null.");
            return this;
        }

        public ProductTourPostStatusBuilder WithStatus(TourStatus status)
        {
            _status = status;
            return this;
        }

        public ProductTourPostStatus Build()
        {
            ValidateBuilder();

            return new ProductTourPostStatus
            {
                Id = _id,
                UserId = _userId,
                TourKey = _tourKey,
                Status = _status
            };
        }

        private void ValidateBuilder()
        {
            if (_userId == null)
                throw new DomainModelArgumentException("UserId is required.");

            if (_tourKey == null)
                throw new DomainModelArgumentException("TourKey is required.");
        }
    }

    #endregion

    #region Business Methods

    public void UpdateStatus(TourStatus status)
    {
        Status = status;
    }

    #endregion
}
