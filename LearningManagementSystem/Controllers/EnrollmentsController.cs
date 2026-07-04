using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PRN232.LMS.API.Helpers;
using PRN232.LMS.API.RequestModels;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.API.Wrappers;
using PRN232.LMS.Models.Response;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.IServices;

using Asp.Versioning;

namespace PRN232.LMS.API.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;


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
        var invalidFields = FieldSelectionHelper.GetInvalidFields(selectedFields, EnrollmentMappingHelper.FieldSelectors);
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
        var enrollments = result.Items.Select(EnrollmentMappingHelper.MapToResponseModel).ToList();
        var response = new PagedResponse<object>(
            success: true,
            message: "Enrollments retrieved successfully.",
            data: FieldSelectionHelper.Apply(enrollments, selectedFields, EnrollmentMappingHelper.FieldSelectors),
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
            data: EnrollmentMappingHelper.MapToResponseModel(enrollment)));
    }

    [HttpPost]
    [Authorize]
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
                data: EnrollmentMappingHelper.MapToResponseModel(createdEnrollment)));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponseModel>>> UpdateEnrollment(
        int id,
        [FromBody] EnrollmentRequestModel request)
    {
        var updatedEnrollment = await _enrollmentService.UpdateEnrollmentAsync(id, new EnrollmentBusinessModel
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status = request.Status
        });

        if (updatedEnrollment == null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Enrollment with id {id} was not found."));
        }

        return Ok(new ApiResponse<EnrollmentResponseModel>(
            success: true,
            message: "Enrollment updated successfully.",
            data: EnrollmentMappingHelper.MapToResponseModel(updatedEnrollment)));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteEnrollment(int id)
    {
        var isDeleted = await _enrollmentService.DeleteEnrollmentAsync(id);

        if (!isDeleted)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Enrollment with id {id} was not found."));
        }

        return Ok(new ApiResponse<object>(
            success: true,
            message: "Enrollment deleted successfully."));
    }
}
