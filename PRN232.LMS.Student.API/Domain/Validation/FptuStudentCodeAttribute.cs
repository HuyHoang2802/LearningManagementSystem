using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PRN232.LMS.Student.API.Domain.Validation
{
    public class FptuStudentCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // [Required] handles empty check if needed
            }

            string studentCode = value.ToString()!;
            // Pattern: Starts with 2 uppercase letters (SE, SA, SS, CE, etc.) followed by 5 digits
            var regex = new Regex(@"^[A-Z]{2}\d{5}$");

            if (!regex.IsMatch(studentCode))
            {
                return new ValidationResult(ErrorMessage ?? "Invalid FPTU Student Code format. Expected format: 2 uppercase letters followed by 5 digits (e.g., SE19886).");
            }

            return ValidationResult.Success;
        }
    }
}
