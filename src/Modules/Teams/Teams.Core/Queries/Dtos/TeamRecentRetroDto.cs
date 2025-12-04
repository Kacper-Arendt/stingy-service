namespace Teams.Core.Queries.Dtos;

public record TeamRecentRetroDto(
    Guid Id,
    string Title,
    DateTime CreatedAt,
    string Status,
    int ParticipantCount,
    int NotesCount)
{
    public static TeamRecentRetroDto FromDatabase(
        Guid id,
        string title,
        DateTime createdAt,
        int status,
        int participantCount,
        int notesCount)
    {
        var statusName = status switch
        {
            0 => "Planned",
            1 => "InProgress", 
            2 => "Revealed",
            3 => "Finished",
            _ => "Unknown"
        };

        return new TeamRecentRetroDto(
            id,
            title,
            createdAt,
            statusName,
            participantCount,
            notesCount);
    }
} 