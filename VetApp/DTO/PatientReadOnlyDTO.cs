namespace VetApp.DTO
{
    public record PatientReadOnlyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ChipNumber { get; set; }
        public string Species { get; set; } = null!;
        public string? Breed { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Veterinarian info (denormalized)
        public int VeterinarianId { get; set; }
        public string VeterinarianFullName { get; set; } = null!;
        public string VeterinarianClinic { get; set; } = null!;

        // Owner info (denormalized)
        public int OwnerId { get; set; }
        public string OwnerFullName { get; set; } = null!;
        public string? OwnerPhoneNumber { get; set; }
    }
}
