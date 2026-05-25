namespace VetApp.DTO
{
    public record VeterinarianReadOnlyDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Clinic { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string UserRole { get; set; } = null!;
    }
}
