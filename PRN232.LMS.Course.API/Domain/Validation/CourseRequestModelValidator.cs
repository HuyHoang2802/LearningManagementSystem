using FluentValidation;
using PRN232.LMS.API.RequestModels;

namespace PRN232.LMS.Course.API.Domain.Validation
{
    public class CourseRequestModelValidator : AbstractValidator<CourseRequestModel>
    {
        public CourseRequestModelValidator()
        {
            RuleFor(x => x.CourseName)
                .NotEmpty().WithMessage("Course name is required.")
                .MaximumLength(100).WithMessage("Course name cannot exceed 100 characters.");

            RuleFor(x => x.SemesterId)
                .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");
        }
    }
}
