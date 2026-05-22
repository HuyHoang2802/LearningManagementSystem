using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helpers;
using PRN232.LMS.API.RequestModels;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.API.Wrappers;
using PRN232.LMS.Models.Response;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.IServices;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;
    private static readonly IReadOnlyDictionary<string, Func<EnrollmentResponseModel, object?>> FieldSelectors =
        new Dictionary<string, Func<EnrollmentResponseModel, object?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["enrollmentId"] = enrollment => enrollment.EnrollmentId,
            ["studentId"] = enrollment => enrollment.StudentId,
            ["courseId"] = enrollment => enrollment.CourseId,
            ["enrollDate"] = enrollment => enrollment.EnrollDate,
            ["status"] = enrollment => enrollment.Status,
            ["student"] = enrollment => enrollment.Student,
            ["course"] = enrollment => enrollment.Course
        };

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<object>>> GetEnrollments(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery(Name = "size")] int size = 10,
        [FromQuery] string? expand = null,
        [FromQuery] string? fields = null)
    {
        var selectedFields = FieldSelectionHelper.ParseFields(fields);
        var invalidFields = FieldSelectionHelper.GetInvalidFields(selectedFields, FieldSelectors);
        if (invalidFields.Count > 0)
        {
            return BadRequest(new ApiResponse<object>(
                success: false,
                message: "One or more requested fields are invalid.",
                errors: new { fields = invalidFields }));
        }

        var result = await _enrollmentService.GetEnrollmentsAsync(
            search,
            sort,
            page,
            size,
            QueryParameterHelper.ParseCommaSeparatedValues(expand));
        var enrollments = result.Items.Select(MapToResponseModel).ToList();
        var response = new PagedResponse<object>(
            success: true,
            message: "Enrollments retrieved successfully.",
            data: FieldSelectionHelper.Apply(enrollments, selectedFields, FieldSelectors),
            pagination: PaginationHelper.Create(result));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponseModel>>> GetEnrollmentById(
        int id,
        [FromQuery] string? expand = null)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(
            id,
            QueryParameterHelper.ParseCommaSeparatedValues(expand));
        if (enrollment is null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Enrollment with id {id} was not found."));
        }

        return Ok(new ApiResponse<EnrollmentResponseModel>(
            success: true,
            message: "Enrollment retrieved successfully.",
            data: MapToResponseModel(enrollment)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponseModel>>> CreateEnrollment(
        [FromBody] EnrollmentRequestModel request)
    {
        var createdEnrollment = await _enrollmentService.CreateEnrollmentAsync(new EnrollmentBusinessModel
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status = request.Status
        });

        return CreatedAtAction(
            nameof(GetEnrollmentById),
            new { id = createdEnrollment.EnrollmentId },
            new ApiResponse<EnrollmentResponseModel>(
                success: true,
                message: "Enrollment created successfully.",
                data: MapToResponseModel(createdEnrollment)));
    }

    private static EnrollmentResponseModel MapToResponseModel(EnrollmentBusinessModel enrollment)
    {
        return new EnrollmentResponseModel
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status,
            Student = enrollment.Student is null ? null : MapStudent(enrollment.Student),
            Course = enrollment.Course is null ? null : MapCourse(enrollment.Course)
        };
    }

    private static StudentResponseModel MapStudent(StudentBusinessModel student)
    {
        return new StudentResponseModel
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth
        };
    }

    private static CourseResponseModel MapCourse(CourseBusinessModel course)
    {
        return new CourseResponseModel
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId
        };
    }
}
