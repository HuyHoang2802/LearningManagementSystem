using System.Text.Json.Serialization;

namespace PRN232.LMS.Models.Response
{
    public class StudentEnrollmentResponse
    {
        [JsonPropertyName("enrollmentId")]
        public int EnrollmentId { get; set; }

        [JsonPropertyName("enrollDate")]
        public DateTime EnrollDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("courseId")]
        public int CourseId { get; set; }

        [JsonPropertyName("courseName")]
        public string? CourseName { get; set; }
    }
}