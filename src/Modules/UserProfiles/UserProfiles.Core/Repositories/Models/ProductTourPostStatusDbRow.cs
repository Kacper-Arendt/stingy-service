namespace UserProfiles.Core.Repositories.Models;

public class ProductTourPostStatusDbRow
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TourKey { get; set; } = string.Empty;
    public int Status { get; set; }
}

