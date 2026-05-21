namespace VetApp.Models;

public class Patient : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ChipNumber { get; set; }
    public string Species { get; set; } = null!;
    public string? Breed { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int VeterinarianId { get; set; }
    public Veterinarian Veterinarian { get; set; } = null!;
    public int OwnerId { get; set; }
    public Owner Owner { get; set; } = null!;
}
