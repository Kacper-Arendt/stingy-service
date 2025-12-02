using UserProfiles.Core.Queries.Dtos;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Queries;

public interface IProductTourQueryService
{
    Task<List<TourStatusDto>> GetAllTourStatusesAsync(UserId userId);
}

