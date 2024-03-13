namespace HaBuddies.DTOs
{
    public class CreateUserDTO
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public List<string>? JoinedEvent { get; set; }
    }
    
    public class EditUserDTO
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
    }

    public class LoginUserDTO
    {
        public required string Email { get; set;}
        public required string Password { get; set; }
    }
}