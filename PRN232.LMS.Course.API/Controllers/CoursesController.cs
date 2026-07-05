using PRN232.LMS.Course.API.Domain.Entities;
using PRN232.LMS.Course.API.Domain.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PRN232.LMS.Course.API.Helpers;
using PRN232.LMS.API.RequestModels;

using PRN232.LMS.Course.API.Domain.Response;
using PRN232.LMS.Course.API.Domain.Response;
using PRN232.LMS.Course.API.Domain.Entities;
using PRN232.LMS.Course.API.Application.Services;



namespace PRN232.LMS.API.Controllers;


[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;
    private static readonly IReadOnlyDictionary<string, Func<CourseResponseModel, object?>> FieldSelectors =
        new Dictionary<string, Func<CourseResponseModel, object?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["courseId"] = course => course.CourseId,
            ["courseName"] = course => course.CourseName,
            ["semesterId"] = course => course.SemesterId,
            ["semester"] = course => course.Semester
        };

    public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;

    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PagedResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<object>>> GetCourses(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery(Name = "size")] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null,
        [FromHeader(Name = "X-Request-Id")] string? requestId = null)
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

        var expands = QueryParameterHelper.ParseCommaSeparatedValues(expand);
        var result = await _courseService.GetCoursesAsync(search, sort, page, size, expands);
        var courses = result.Items.Select(MapToResponseModel).ToList();
        var response = new PagedResponse<object>(
            success: true,
            message: "Courses retrieved successfully.",
            data: FieldSelectionHelper.Apply(courses, selectedFields, FieldSelectors),
            pagination: PaginationHelper.Create(result));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseResponseModel>>> GetCourseById(
        int id,
        [FromQuery] string? expand = null)
    {
        var course = await _courseService.GetCourseByIdAsync(
            id,
            QueryParameterHelper.ParseCommaSeparatedValues(expand));
        if (course is null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Course with id {id} was not found."));
        }

        return Ok(new ApiResponse<CourseResponseModel>(
            success: true,
            message: "Course retrieved successfully.",
            data: MapToResponseModel(course)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CourseResponseModel>>> CreateCourse(
        [FromBody] CourseRequestModel request)
    {
        var createdCourse = await _courseService.CreateCourseAsync(new CourseBusinessModel
        {
            CourseName = request.CourseName,
            SemesterId = request.SemesterId
        });

        return CreatedAtAction(
            nameof(GetCourseById),
            new { id = createdCourse.CourseId },
            new ApiResponse<CourseResponseModel>(
                success: true,
                message: "Course created successfully.",
                data: MapToResponseModel(createdCourse)));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseResponseModel>>> UpdateCourse(
        int id,
        [FromBody] CourseRequestModel request)
    {
        var updatedCourse = await _courseService.UpdateCourseAsync(id, new CourseBusinessModel
        {
            CourseName = request.CourseName,
            SemesterId = request.SemesterId
        });

        if (updatedCourse == null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Course with id {id} was not found."));
        }

        return Ok(new ApiResponse<CourseResponseModel>(
            success: true,
            message: "Course updated successfully.",
            data: MapToResponseModel(updatedCourse)));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCourse(int id)
    {
        var isDeleted = await _courseService.DeleteCourseAsync(id);

        if (!isDeleted)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Course with id {id} was not found."));
        }

        return Ok(new ApiResponse<object>(
            success: true,
            message: "Course deleted successfully."));
    }

    private static CourseResponseModel MapToResponseModel(CourseBusinessModel course)
    {
        return new CourseResponseModel
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId,
            Semester = course.Semester is null ? null : MapSemester(course.Semester)
        };
    }

    private static SemesterResponseModel MapSemester(SemesterBusinessModel semester)
    {
        return new SemesterResponseModel
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate
        };
    }
    [HttpGet("{id:int}/enrollments")]
    [ProducesResponseType(typeof(PagedResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<object>>> GetCourseEnrollments(
     int id,
     [FromQuery] string? search,
     [FromQuery] string? sort,
     [FromQuery] int page = 1,
     [FromQuery(Name = "size")] int size = 10,
     [FromQuery] string? fields = null,
     [FromQuery] string? expand = null)
    {
        var selectedFields = FieldSelectionHelper.ParseFields(fields);
       
        var invalidFields = FieldSelectionHelper.GetInvalidFields(selectedFields, EnrollmentMappingHelper.FieldSelectors);

        if (invalidFields.Count > 0)
        {
            return BadRequest(new ApiResponse<object>(
                success: false,
                message: "One or more requested fields are invalid.",
                errors: new { fields = invalidFields }));
        }

        var expands = QueryParameterHelper.ParseCommaSeparatedValues(expand);
        var result = await _enrollmentService.GetEnrollmentsAsync(search, sort, page, size, expands, courseId: id);

        var enrollments = result.Items.Select(EnrollmentMappingHelper.MapToResponseModel).ToList();

        var response = new PagedResponse<object>(
            success: true,
            message: "Course enrollments retrieved successfully.",
          
            data: FieldSelectionHelper.Apply(enrollments, selectedFields, EnrollmentMappingHelper.FieldSelectors),
            pagination: PaginationHelper.Create(result));

        return Ok(response);
    }
}
