using UserProfiles.Core.Commands.Dtos;
using UserProfiles.Core.Repositories;
using UserProfiles.Domain.Entities;
using UserProfiles.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Commands;

public class ProductTourCommandService : IProductTourCommandService
{
    private readonly IProductTourPostStatusRepository _repository;

    public ProductTourCommandService(IProductTourPostStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task SetTourStatusAsync(UserId userId, SetTourStatusDto dto)
    {
        var tourKey = new TourKey(dto.TourKey);
        
        var existingStatus = await _repository.GetByUserAndTourKeyAsync(userId, tourKey);

        if (existingStatus != null)
        {
            existingStatus.UpdateStatus(dto.Status);
            await _repository.UpsertAsync(existingStatus);
        }
        else
        {
            var newStatus = new ProductTourPostStatus.ProductTourPostStatusBuilder()
                .WithUserId(userId)
                .WithTourKey(tourKey)
                .WithStatus(dto.Status)
                .Build();

            await _repository.UpsertAsync(newStatus);
        }
    }
}

