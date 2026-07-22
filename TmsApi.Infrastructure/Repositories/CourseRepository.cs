using Microsoft.EntityFrameworkCore;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Repositories;

public class CourseRepository(TmsDbContext db) : ICourseRepository
{
    public Task<bool> CodeExistsAsync(string Code, CancellationToken ct) => db.Courses.AsNoTracking().AnyAsync(c => c.Code == Code, ct);

    public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct)
    {

        IQueryable<Course> query = db.Courses.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.Title, $"%{request.Search}%") ||
                EF.Functions.ILike(c.Code, $"%{request.Search}%"));
        }

        var totalCount = await query.CountAsync(ct);

        IQueryable<Course> sortedQuery = request.OrderBy switch
        {
            "Code" => request.Descending
                ? query.OrderByDescending(c => c.Code)
                : query.OrderBy(c => c.Code),

            "MaxCapacity" => request.Descending
                ? query.OrderByDescending(c => c.MaxCapacity)
                : query.OrderBy(c => c.MaxCapacity),

            _ => request.Descending
                ? query.OrderByDescending(c => c.Title)
                : query.OrderBy(c => c.Title),
        };

        var items = await sortedQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Code,
                c.Title,
                c.MaxCapacity,
                c.Enrollments.Count
                ))
            .ToListAsync(ct);

        return new PagedResponse<CourseResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<Course?> GetCourseEntityByCodeAsync(string code, CancellationToken ct)
    {
        return await db.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Code == code, ct);
    }
    public async Task<List<Course>> GetAllAsync(CancellationToken ct)
    {
        return await db.Courses
            .Include(c => c.Enrollments)
            .ToListAsync(ct);
    }

    public async Task<Course?> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        return await db.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Course?> GetByCodeAsync(
    string code,
    CancellationToken ct)
    {
        return await db.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Code == code, ct);
    }

    public async Task AddAsync(Course course, CancellationToken ct)
    {
        await db.Courses.AddAsync(course, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Course course, CancellationToken ct)
    {
        db.Courses.Update(course);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Course course, CancellationToken ct)
    {
        db.Courses.Remove(course);
        await db.SaveChangesAsync(ct);
    }

}