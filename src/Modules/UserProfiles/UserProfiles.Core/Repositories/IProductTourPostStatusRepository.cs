using UserProfiles.Domain.Entities;
using UserProfiles.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Repositories;

public interface IProductTourPostStatusRepository
{
    Task<ProductTourPostStatus?> GetByUserAndTourKeyAsync(UserId userId, TourKey tourKey);
    Task<List<ProductTourPostStatus>> GetAllByUserIdAsync(UserId userId);
    Task UpsertAsync(ProductTourPostStatus tourStatus);
}