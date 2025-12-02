using UserProfiles.Core.Commands.Dtos;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Commands;

public interface IProductTourCommandService
{
    Task SetTourStatusAsync(UserId userId, SetTourStatusDto dto);
}

