using UserProfiles.Core.Queries.Dtos;
using UserProfiles.Core.Repositories;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Queries;

public class ProductTourQueryService : IProductTourQueryService
{
    private readonly IProductTourPostStatusRepository _repository;

    public ProductTourQueryService(IProductTourPostStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TourStatusDto>> GetAllTourStatusesAsync(UserId userId)
    {
        var tourStatuses = await _repository.GetAllByUserIdAsync(userId);
        
        return [.. tourStatuses.Select(ts => new TourStatusDto
        {
            TourKey = ts.TourKey.Value,
            Status = ts.Status
        })];
    }
}

