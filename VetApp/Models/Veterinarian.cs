namespace VetApp.Models;

public class Veterinarian : BaseEntity
{
    public int Id { get; set; }
    public string Clinic { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public int UserId { get; set; }
    public ICollection<Patient> Patients { get; set; } = new HashSet<Patient>();
    public User User { get; set; } = null!;
}
