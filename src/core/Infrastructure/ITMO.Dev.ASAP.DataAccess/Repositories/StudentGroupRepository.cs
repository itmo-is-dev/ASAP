using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Mapping;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ITMO.Dev.ASAP.DataAccess.Repositories;

public class StudentGroupRepository : IStudentGroupRepository
{
    private readonly DatabaseContext _context;

    public StudentGroupRepository(DatabaseContext context)
    {
        _context = context;
    }

    public IAsyncEnumerable<StudentGroup> QueryAsync(StudentGroupQuery query, CancellationToken cancellationToken)
    {
        IQueryable<StudentGroupModel> queryable = _context.StudentGroups;

        if (query.Ids is not [])
        {
            queryable = queryable.Where(x => query.Ids.Contains(x.Id));
        }

        if (query.StudentIds is not [])
        {
            queryable = queryable.Where(g => g.Students
                .Select(x => x.UserId)
                .Any(x => query.StudentIds.Contains(x)));
        }

        if (query.NamePattern is not null)
        {
            queryable = queryable.Where(x => EF.Functions.ILike(x.Name, query.NamePattern));
        }

        var finalQueryable = queryable.Select(studentGroup => new
        {
            studentGroup,
            students = studentGroup.Students.Select(x => x.UserId),
        });

        return finalQueryable.AsAsyncEnumerable().Select(x => MapTo(x.studentGroup, x.students));
    }

    public void Update(StudentGroup studentGroup)
    {
        EntityEntry<StudentGroupModel> entry = GetEntry(
            studentGroup.Id,
            () => StudentGroupMapper.MapFrom(studentGroup));

        StudentGroupModel model = entry.Entity;
        model.Name = studentGroup.Name;

        entry.State = EntityState.Modified;
    }

    public void Add(StudentGroup studentGroup)
    {
        StudentGroupModel model = StudentGroupMapper.MapFrom(studentGroup);
        _context.StudentGroups.Add(model);
    }

    private static StudentGroup MapTo(StudentGroupModel model, IEnumerable<Guid> studentIds)
    {
        return StudentGroupMapper.MapTo(model, studentIds.ToHashSet());
    }

    private EntityEntry<StudentGroupModel> GetEntry(Guid studentGroupId, Func<StudentGroupModel> modelFactory)
    {
        StudentGroupModel? existing = _context.StudentGroups.Local
            .FirstOrDefault(model => model.Id.Equals(studentGroupId));

        return existing is not null
            ? _context.Entry(existing)
            : _context.StudentGroups.Attach(modelFactory.Invoke());
    }
}