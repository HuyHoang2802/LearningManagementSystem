using PRN232.LMS.Student.API.Domain.Entities;
using PRN232.LMS.Student.API.Domain.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PRN232.LMS.Student.API.Helpers;
using PRN232.LMS.Student.API.Domain.Request;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.Student.API.Domain.Response;
using PRN232.LMS.Student.API.Domain.Response;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Student.API.Application.Services;



namespace PRN232.LMS.API.Controllers;


[ApiController]
[Route("api/subjects")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;
    private static readonly IReadOnlyDictionary<string, Func<SubjectResponseModel, object?>> FieldSelectors =
        new Dictionary<string, Func<SubjectResponseModel, object?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["subjectId"] = subject => subject.SubjectId,
            ["subjectCode"] = subject => subject.SubjectCode,
            ["subjectName"] = subject => subject.SubjectName,
            ["credit"] = subject => subject.Credit
        };

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PagedResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<object>>> GetSubjects(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery(Name = "size")] int size = 10,
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

        var result = await _subjectService.GetSubjectsAsync(search, sort, page, size);
        var subjects = result.Items.Select(MapToResponseModel).ToList();
        var response = new PagedResponse<object>(
            success: true,
            message: "Subjects retrieved successfully.",
            data: FieldSelectionHelper.Apply(subjects, selectedFields, FieldSelectors),
            pagination: PaginationHelper.Create(result));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SubjectResponseModel>>> GetSubjectById(int id)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id);
        if (subject is null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Subject with id {id} was not found."));
        }

        return Ok(new ApiResponse<SubjectResponseModel>(
            success: true,
            message: "Subject retrieved successfully.",
            data: MapToResponseModel(subject)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SubjectResponseModel>>> CreateSubject(
        [FromBody] SubjectRequestModel request)
    {
        var createdSubject = await _subjectService.CreateSubjectAsync(new SubjectBusinessModel
        {
            SubjectCode = request.SubjectCode,
            SubjectName = request.SubjectName,
            Credit = request.Credit
        });

        return CreatedAtAction(
            nameof(GetSubjectById),
            new { id = createdSubject.SubjectId },
            new ApiResponse<SubjectResponseModel>(
                success: true,
                message: "Subject created successfully.",
                data: MapToResponseModel(createdSubject)));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SubjectResponseModel>>> UpdateSubject(
        int id,
        [FromBody] SubjectRequestModel request)
    {
        var updatedSubject = await _subjectService.UpdateSubjectAsync(id, new SubjectBusinessModel
        {
            SubjectCode = request.SubjectCode,
            SubjectName = request.SubjectName,
            Credit = request.Credit
        });

        if (updatedSubject == null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Subject with id {id} was not found."));
        }

        return Ok(new ApiResponse<SubjectResponseModel>(
            success: true,
            message: "Subject updated successfully.",
            data: MapToResponseModel(updatedSubject)));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSubject(int id)
    {
        var isDeleted = await _subjectService.DeleteSubjectAsync(id);

        if (!isDeleted)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Subject with id {id} was not found."));
        }

        return Ok(new ApiResponse<object>(
            success: true,
            message: "Subject deleted successfully."));
    }

    private static SubjectResponseModel MapToResponseModel(SubjectBusinessModel subject)
    {
        return new SubjectResponseModel
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credit = subject.Credit
        };
    }
}
