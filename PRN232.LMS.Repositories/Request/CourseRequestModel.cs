using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.RequestModels;

public class CourseRequestModel
{
    [Required]
    [MaxLength(100)]
    public string CourseName { get; set; } = string.Empty;

    public int SemesterId { get; set; }
    public int EnrollmentId { get; set; }
}
