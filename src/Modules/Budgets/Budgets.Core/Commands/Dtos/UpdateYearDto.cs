using Budgets.Domain.Enums;

namespace Budgets.Core.Commands.Dtos;

public class UpdateYearDto
{
    public int? Value { get; set; }

    public YearStatus? Status { get; set; }
}
