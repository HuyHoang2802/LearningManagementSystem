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
[Route("api/students")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;
    private static readonly IReadOnlyDictionary<string, Func<StudentResponseModel, object?>> FieldSelectors =
        new Dictionary<string, Func<StudentResponseModel, object?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["studentId"] = student => student.StudentId,
            ["fullName"] = student => student.FullName,
            ["email"] = student => student.Email,
            ["dateOfBirth"] = student => student.DateOfBirth,
            ["enrollments"] = student => student.Enrollments
        };

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<object>>> GetStudents([FromQuery] StudentQueryRequest query)
    {
        var selectedFields = FieldSelectionHelper.ParseFields(query.Fields);
        var invalidFields = FieldSelectionHelper.GetInvalidFields(selectedFields, FieldSelectors);
        if (invalidFields.Count > 0)
        {
            return BadRequest(new ApiResponse<object>(
                success: false,
                message: "One or more requested fields are invalid.",
                errors: new { fields = invalidFields }));
        }

        var expands = QueryParameterHelper.ParseCommaSeparatedValues(query.Expand);
        var result = await _studentService.GetStudentsAsync(query.Search, query.Sort, query.Page, query.Size, expands);
        var students = result.Items.Select(s => MapToResponseModel(s, expands)).ToList();

        var response = new PagedResponse<object>(
            success: true,
            message: "Students retrieved successfully.",
            data: FieldSelectionHelper.Apply(students, selectedFields, FieldSelectors),
            pagination: PaginationHelper.Create(result));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentResponseModel>>> GetStudentById(
        int id,
        [FromQuery] string? expand = null)
    {
        var expands = QueryParameterHelper.ParseCommaSeparatedValues(expand);
        var student = await _studentService.GetStudentByIdAsync(id, expands);
        if (student is null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Student with id {id} was not found."));
        }

        return Ok(new ApiResponse<StudentResponseModel>(
            success: true,
            message: "Student retrieved successfully.",
            data: MapToResponseModel(student, expands)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<StudentResponseModel>>> CreateStudent(
        [FromBody] CreateStudentRequest request)
    {
        var createdStudent = await _studentService.CreateStudentAsync(new StudentBusinessModel
        {
            FullName = request.FullName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth
        });

        return CreatedAtAction(
            nameof(GetStudentById),
            new { id = createdStudent.StudentId },
            new ApiResponse<StudentResponseModel>(
                success: true,
                message: "Student created successfully.",
                data: MapToResponseModel(createdStudent, new List<string>())));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentResponseModel>>> UpdateStudent(
        int id,
        [FromBody] UpdateStudentRequest request)
    {
        var updatedStudent = await _studentService.UpdateStudentAsync(id, new StudentBusinessModel
        {
            FullName = request.FullName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth
        });

        if (updatedStudent is null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Student with id {id} was not found."));
        }

        return Ok(new ApiResponse<StudentResponseModel>(
            success: true,
            message: "Student updated successfully.",
            data: MapToResponseModel(updatedStudent, new List<string>())));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteStudent(int id)
    {
        var result = await _studentService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Student with id {id} was not found."));
        }

        return Ok(new ApiResponse<bool>(
            success: true,
            message: "Student deleted successfully.",
            data: true));
    }

    private static StudentResponseModel MapToResponseModel(StudentBusinessModel student, List<string> expands)
    {
        return new StudentResponseModel
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth,
            Enrollments = student.Enrollments?.Select(MapEnrollment).ToList()
        };
    }

    private static EnrollmentResponseModel MapEnrollment(EnrollmentBusinessModel enrollment)
    {
        return new EnrollmentResponseModel
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status,
            Course = enrollment.Course is null ? null : new CourseResponseModel
            {
                CourseId = enrollment.Course.CourseId,
                CourseName = enrollment.Course.CourseName,
                SemesterId = enrollment.Course.SemesterId
            }
        };
    }
}