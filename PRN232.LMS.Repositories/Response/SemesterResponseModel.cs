namespace PRN232.LMS.API.ResponseModels;

public class SemesterResponseModel
{
    public int SemesterId { get; set; }

    public string SemesterName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
