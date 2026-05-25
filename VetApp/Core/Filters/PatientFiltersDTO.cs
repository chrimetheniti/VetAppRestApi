namespace VetApp.Core.Filters
{
    public class PatientFiltersDTO
    {
        public string? Name { get; set; }
        public string? ChipNumber { get; set; }
        public string? Species { get; set; }
        public string? Breed { get; set; }
        public int? VeterinarianId { get; set; }
        public int? OwnerId { get; set; }
    }
}
