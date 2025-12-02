using UserProfiles.Domain.Enums;

namespace UserProfiles.Core.Queries.Dtos;

public class TourStatusDto
{
    public string TourKey { get; set; } = string.Empty;
    public TourStatus Status { get; set; }
}

