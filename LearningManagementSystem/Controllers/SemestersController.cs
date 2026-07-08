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
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;
    private static readonly IReadOnlyDictionary<string, Func<SemesterResponseModel, object?>> FieldSelectors =
        new Dictionary<string, Func<SemesterResponseModel, object?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["semesterId"] = semester => semester.SemesterId,
            ["semesterName"] = semester => semester.SemesterName,
            ["startDate"] = semester => semester.StartDate,
            ["endDate"] = semester => semester.EndDate
        };

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<object>>> GetSemesters(
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

        var result = await _semesterService.GetSemestersAsync(search, sort, page, size);
        var semesters = result.Items.Select(MapToResponseModel).ToList();
        var response = new PagedResponse<object>(
            success: true,
            message: "Semesters retrieved successfully.",
            data: FieldSelectionHelper.Apply(semesters, selectedFields, FieldSelectors),
            pagination: PaginationHelper.Create(result));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SemesterResponseModel>>> GetSemesterById(int id)
    {
        var semester = await _semesterService.GetSemesterByIdAsync(id);
        if (semester is null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Semester with id {id} was not found."));
        }

        return Ok(new ApiResponse<SemesterResponseModel>(
            success: true,
            message: "Semester retrieved successfully.",
            data: MapToResponseModel(semester)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SemesterResponseModel>>> CreateSemester(
        [FromBody] SemesterRequestModel request)
    {
        var createdSemester = await _semesterService.CreateSemesterAsync(new SemesterBusinessModel
        {
            SemesterName = request.SemesterName,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        });

        return CreatedAtAction(
            nameof(GetSemesterById),
            new { id = createdSemester.SemesterId },
            new ApiResponse<SemesterResponseModel>(
                success: true,
                message: "Semester created successfully.",
                data: MapToResponseModel(createdSemester)));
    }

    private static SemesterResponseModel MapToResponseModel(SemesterBusinessModel semester)
    {
        return new SemesterResponseModel
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate
        };
    }
}
