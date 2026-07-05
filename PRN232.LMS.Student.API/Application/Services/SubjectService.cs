using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Student.API.Domain.Entities;
using PRN232.LMS.Student.API.Infrastructure.Repositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Student.API.Application.Services;

namespace PRN232.LMS.Student.API.Application.Services;

public class SubjectService : ISubjectService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork _unitOfWork;

    public SubjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultModel<SubjectBusinessModel>> GetSubjectsAsync(
        string? search,
        string? sort,
        int page,
        int size)
    {
        var normalizedPage = page <= 0 ? DefaultPage : page;
        var normalizedSize = size <= 0 ? DefaultPageSize : Math.Min(size, MaxPageSize);
        var (sortBy, sortDescending) = NormalizeSort(sort);

        var query = ApplySearch(_unitOfWork.Subjects.GetQueryable(), search);
        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var subjects = await ApplySort(query, sortBy, sortDescending)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<SubjectBusinessModel>
        {
            Items = subjects.Select(MapToBusinessModel).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<SubjectBusinessModel?> GetSubjectByIdAsync(int id)
    {
        var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
        return subject is null ? null : MapToBusinessModel(subject);
    }

    public async Task<SubjectBusinessModel> CreateSubjectAsync(SubjectBusinessModel subject)
    {
        var entity = new Subject
        {
            Subjectcode = subject.SubjectCode,
            Subjectname = subject.SubjectName,
            Credit = subject.Credit
        };

        await _unitOfWork.Subjects.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return MapToBusinessModel(entity);
    }

    public async Task<SubjectBusinessModel?> UpdateSubjectAsync(int id, SubjectBusinessModel subject)
    {
        var entity = await _unitOfWork.Subjects.GetQueryable().FirstOrDefaultAsync(s => s.Subjectid == id);
        if (entity == null) return null;

        entity.Subjectcode = subject.SubjectCode;
        entity.Subjectname = subject.SubjectName;
        entity.Credit = subject.Credit;

        await _unitOfWork.Subjects.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return MapToBusinessModel(entity);
    }

    public async Task<bool> DeleteSubjectAsync(int id)
    {
        var entity = await _unitOfWork.Subjects.GetQueryable().FirstOrDefaultAsync(s => s.Subjectid == id);
        if (entity == null) return false;

        await _unitOfWork.Subjects.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static IQueryable<Subject> ApplySearch(IQueryable<Subject> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim();
        return query.Where(subject =>
            EF.Functions.ILike(subject.Subjectname, $"%{keyword}%") ||
            EF.Functions.ILike(subject.Subjectcode, $"%{keyword}%"));
    }

    private static IQueryable<Subject> ApplySort(
        IQueryable<Subject> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy switch
        {
            "subjectCode" => sortDescending
                ? query.OrderByDescending(subject => subject.Subjectcode)
                : query.OrderBy(subject => subject.Subjectcode),
            "credit" => sortDescending
                ? query.OrderByDescending(subject => subject.Credit)
                : query.OrderBy(subject => subject.Credit),
            _ => sortDescending
                ? query.OrderByDescending(subject => subject.Subjectname)
                : query.OrderBy(subject => subject.Subjectname)
        };
    }

    private static (string SortBy, bool SortDescending) NormalizeSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return ("subjectName", false);
        }

        var trimmedSort = sort.Trim();
        var sortDescending = trimmedSort.StartsWith('-');
        var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

        return sortBy switch
        {
            "subjectCode" => (sortBy, sortDescending),
            "subjectName" => (sortBy, sortDescending),
            "credit" => (sortBy, sortDescending),
            _ => ("subjectName", false)
        };
    }

    private static SubjectBusinessModel MapToBusinessModel(Subject subject)
    {
        return new SubjectBusinessModel
        {
            SubjectId = subject.Subjectid,
            SubjectCode = subject.Subjectcode,
            SubjectName = subject.Subjectname,
            Credit = subject.Credit
        };
    }
}
