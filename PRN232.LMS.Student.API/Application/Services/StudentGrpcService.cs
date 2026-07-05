using Grpc.Core;
using System.Threading.Tasks;
using PRN232.LMS.Student.API.Infrastructure.Repositories;
using PRN232.LMS.Student.API.Domain.Entities;

namespace PRN232.LMS.Student.API.Application.Services
{
    public class StudentGrpcService : Protos.StudentGrpc.StudentGrpcBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentGrpcService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Protos.StudentResponse> CheckStudentExists(Protos.StudentRequest request, ServerCallContext context)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId);
            return new Protos.StudentResponse
            {
                Exists = student != null,
                Fullname = student?.Fullname ?? string.Empty,
                Email = student?.Email ?? string.Empty
            };
        }

        public override async Task<Protos.StudentResponse> GetStudentById(Protos.StudentRequest request, ServerCallContext context)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId);
            return new Protos.StudentResponse
            {
                Exists = student != null,
                Fullname = student?.Fullname ?? string.Empty,
                Email = student?.Email ?? string.Empty
            };
        }
    }
}