using PRN232.LMS.Student.API.Domain.Entities;
using PRN232.LMS.Student.API.Domain.Response;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Student.API.Domain.Request;
using PRN232.LMS.Student.API.Domain.Response;
using PRN232.LMS.Student.API.Application.Services;
using Microsoft.AspNetCore.Authorization;


namespace PRN232.LMS.API.Controllers
{
    
    
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] StudentQueryRequest query)
        {
            var result = await _studentService.GetStudentsAsync(query);
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _studentService.GetStudentByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
        {
            var result = await _studentService.CreateStudentAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.StudentId }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStudentRequest request)
        {
            var result = await _studentService.UpdateStudentAsync(id, request);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _studentService.DeleteAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}