namespace PRN232.LMS.Course.API.Domain.Response;

public class CourseResponseModel
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;
   
    public int SemesterId { get; set; }

    public SemesterResponseModel? Semester { get; set; }
}
