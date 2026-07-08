using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.RequestModels;

public class SubjectRequestModel
{
    [Required]
    [MaxLength(20)]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SubjectName { get; set; } = string.Empty;

    public int Credit { get; set; }
}
