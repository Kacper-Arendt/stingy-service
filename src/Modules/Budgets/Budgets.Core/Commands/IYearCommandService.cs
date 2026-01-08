using Budgets.Core.Commands.Dtos;
using Budgets.Core.Queries.Dtos;
using Budgets.Domain.ValueObjects;

namespace Budgets.Core.Commands;

public interface IYearCommandService
{
    Task<YearDto> CreateAsync(CreateYearDto dto);
    Task<YearDto> UpdateAsync(YearId yearId, UpdateYearDto dto);
    Task ArchiveAsync(YearId yearId);
    Task DeleteAsync(YearId yearId);
}
