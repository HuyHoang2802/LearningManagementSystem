using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Student.API.Domain.Entities;
using PRN232.LMS.Student.API.Infrastructure.Repositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Student.API.Application.Services;

namespace PRN232.LMS.Student.API.Application.Services;

public class SemesterService : ISemesterService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork _unitOfWork;

    public SemesterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultModel<SemesterBusinessModel>> GetSemestersAsync(
        string? search,
        string? sort,
        int page,
        int size)
    {
        var normalizedPage = page <= 0 ? DefaultPage : page;
        var normalizedSize = size <= 0 ? DefaultPageSize : Math.Min(size, MaxPageSize);
        var (sortBy, sortDescending) = NormalizeSort(sort);

        var query = ApplySearch(_unitOfWork.Semesters.GetQueryable(), search);
        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var semesters = await ApplySort(query, sortBy, sortDescending)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<SemesterBusinessModel>
        {
            Items = semesters.Select(MapToBusinessModel).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<SemesterBusinessModel?> GetSemesterByIdAsync(int id)
    {
        var semester = await _unitOfWork.Semesters.GetByIdAsync(id);
        return semester is null ? null : MapToBusinessModel(semester);
    }

    public async Task<SemesterBusinessModel> CreateSemesterAsync(SemesterBusinessModel semester)
    {
        var entity = new Semester
        {
            Semestername = semester.SemesterName,
            Startdate = DateOnly.FromDateTime(semester.StartDate),
            Enddate = DateOnly.FromDateTime(semester.EndDate)
        };

        await _unitOfWork.Semesters.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return MapToBusinessModel(entity);
    }

    public async Task<SemesterBusinessModel?> UpdateSemesterAsync(int id, SemesterBusinessModel semester)
    {
        var entity = await _unitOfWork.Semesters.GetQueryable().FirstOrDefaultAsync(s => s.Semesterid == id);
        if (entity == null) return null;

        entity.Semestername = semester.SemesterName;
        entity.Startdate = DateOnly.FromDateTime(semester.StartDate);
        entity.Enddate = DateOnly.FromDateTime(semester.EndDate);

        await _unitOfWork.Semesters.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return MapToBusinessModel(entity);
    }

    public async Task<bool> DeleteSemesterAsync(int id)
    {
        var entity = await _unitOfWork.Semesters.GetQueryable().FirstOrDefaultAsync(s => s.Semesterid == id);
        if (entity == null) return false;

        await _unitOfWork.Semesters.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static IQueryable<Semester> ApplySearch(IQueryable<Semester> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim();
        return query.Where(semester => EF.Functions.ILike(semester.Semestername, $"%{keyword}%"));
    }

    private static IQueryable<Semester> ApplySort(
        IQueryable<Semester> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy switch
        {
            "startDate" => sortDescending
                ? query.OrderByDescending(semester => semester.Startdate)
                : query.OrderBy(semester => semester.Startdate),
            "endDate" => sortDescending
                ? query.OrderByDescending(semester => semester.Enddate)
                : query.OrderBy(semester => semester.Enddate),
            _ => sortDescending
                ? query.OrderByDescending(semester => semester.Semestername)
                : query.OrderBy(semester => semester.Semestername)
        };
    }

    private static (string SortBy, bool SortDescending) NormalizeSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return ("semesterName", false);
        }

        var trimmedSort = sort.Trim();
        var sortDescending = trimmedSort.StartsWith('-');
        var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

        return sortBy switch
        {
            "startDate" => (sortBy, sortDescending),
            "endDate" => (sortBy, sortDescending),
            "semesterName" => (sortBy, sortDescending),
            _ => ("semesterName", false)
        };
    }

    private static SemesterBusinessModel MapToBusinessModel(Semester semester)
    {
        return new SemesterBusinessModel
        {
            SemesterId = semester.Semesterid,
            SemesterName = semester.Semestername,
            StartDate = semester.Startdate.ToDateTime(TimeOnly.MinValue),
            EndDate = semester.Enddate.ToDateTime(TimeOnly.MinValue)
        };
    }
}
