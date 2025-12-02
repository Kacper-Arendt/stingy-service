namespace UserProfiles.Shared.Dtos;

public class UserDisplayInfoDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ShortDisplayName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public bool HasProfile { get; set; }

    public static string GenerateShortDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return "??";

        var words = displayName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (words.Length >= 2)
        {
            // First letter of first two words
            return $"{words[0][0]}{words[1][0]}".ToUpper();
        }
        
        if (words[0].Length >= 2)
        {
            // First two letters of single word
            return words[0].Substring(0, 2).ToUpper();
        }
        
        // Single letter + ?
        return $"{words[0][0]}?".ToUpper();
    }
}
