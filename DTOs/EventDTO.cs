namespace HaBuddies.DTOs
{
    public class CreateEventDTO
    {
        public required string Title { get; set; }
        // public required string OwnerId { get; set; } 
        public required string Category { get; set; }
        public required int EnrollSize { get; set; }
        public string? Description { get; set; }
        public required DateTime EndDate { get; set; }
        public int? MinAgeRequirement { get; set; }
        public int? MaxAgeRequirement { get; set; }
        public List<string>? GenderRequirement { get; set; }
    }

    public class EditEventDTO
    {
        public string? Title { get; set; }
        public string? Category { get; set; }
        public int? EnrollSize { get; set; }
        public string? Description { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MinAgeRequirement { get; set; }
        public int? MaxAgeRequirement { get; set; }
        public List<string>? GenderRequirement { get; set; }
        public bool? IsOpen { get; set; }
    }
}