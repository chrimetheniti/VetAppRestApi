using System.ComponentModel.DataAnnotations;

namespace VetApp.DTO
{
    public record PatientInsertDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 50 characters.")]
        public string? Name { get; set; }

        [StringLength(15, MinimumLength = 15, ErrorMessage = "Chip number must be exactly 15 digits.")]
        [RegularExpression(@"^\d{15}$", ErrorMessage = "Chip number must contain only digits (15 digits).")]
        public string? ChipNumber { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Species must be between 2 and 50 characters.")]
        public string? Species { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Breed must be between 2 and 50 characters.")]
        public string? Breed { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public int? VeterinarianId { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public int? OwnerId { get; set; }
    }
}
