namespace VetApp.Models;

public class Owner : BaseEntity
{
    public int Id { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Patient> Patients { get; set; } = new HashSet<Patient>();
}
