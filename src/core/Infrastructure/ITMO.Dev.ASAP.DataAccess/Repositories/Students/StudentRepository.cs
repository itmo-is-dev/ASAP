using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Mapping;
using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Domain.Students;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.DataAccess.Repositories.Students;

public class StudentRepository : RepositoryBase<Student, StudentModel>, IStudentRepository
{
    private readonly DatabaseContext _context;
    private readonly IStudentEventVisitor _visitor;

    public StudentRepository(DatabaseContext context) : base(context)
    {
        _context = context;
        _visitor = new StudentRepositoryEventVisitor(context);
    }

    protected override DbSet<StudentModel> DbSet => _context.Students;

    public IAsyncEnumerable<Student> QueryAsync(StudentQuery query, CancellationToken cancellationToken)
    {
        IQueryable<StudentModel> queryable = DbSet;

        if (query.Ids is not [])
        {
            queryable = queryable.Where(x => query.Ids.Contains(x.UserId));
        }

        if (query.GroupIds is not [])
        {
            queryable = queryable
                .Where(x => query.GroupIds.Contains(x.StudentGroupId!.Value));
        }

        if (query.AssignmentIds is not [])
        {
            queryable = queryable
                .Where(student => student.StudentGroup!.SubjectCourseGroups
                    .SelectMany(x => x.SubjectCourse.Assignments)
                    .Select(x => x.Id)
                    .Any(x => query.AssignmentIds.Contains(x)));
        }

        if (query.SubjectCourseIds is not [])
        {
            queryable = queryable
                .Where(student => student.StudentGroup!.SubjectCourseGroups
                    .Any(x => query.SubjectCourseIds.Contains(x.SubjectCourse.Id)));
        }

        if (query.GroupNamePattern is not null)
        {
            queryable = queryable
                .Where(x => EF.Functions.ILike(x.StudentGroup!.Name, query.GroupNamePattern));
        }

        if (query.UniversityIdPattern is not null)
        {
            queryable = queryable
                .Select(x => new
                {
                    student = x,
                    association = x.User.Associations.OfType<IsuUserAssociationModel>().SingleOrDefault(),
                })
                .Where(x => EF.Functions.ILike(x.association!.UniversityId.ToString(), query.UniversityIdPattern))
                .Select(x => x.student);
        }

        if (query.FullNamePattern is not null)
        {
            queryable = queryable.Where(student =>
                EF.Functions.ILike(student.User.FirstName, query.FullNamePattern)
                || EF.Functions.ILike(student.User.MiddleName, query.FullNamePattern)
                || EF.Functions.ILike(student.User.LastName, query.FullNamePattern));
        }

        queryable = queryable
            .Include(x => x.User)
            .ThenInclude(x => x.Associations);

        return queryable.AsAsyncEnumerable().Select(StudentMapper.MapTo);
    }

    public ValueTask ApplyAsync(IStudentEvent evt, CancellationToken cancellationToken)
    {
        return evt.AcceptAsync(_visitor, cancellationToken);
    }

    protected override StudentModel MapFrom(Student entity)
    {
        return StudentMapper.MapFrom(entity);
    }

    protected override bool Equal(Student entity, StudentModel model)
    {
        return entity.UserId.Equals(model.UserId);
    }

    protected override void UpdateModel(StudentModel model, Student entity)
    {
        model.StudentGroupId = entity.Group?.Id;
    }
}