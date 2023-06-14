using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Mapping;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.DataAccess.Tools;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Students.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ITMO.Dev.ASAP.DataAccess.Repositories.Students;

public class StudentRepositoryEventVisitor : IStudentEventVisitor
{
    private readonly DatabaseContext _context;

    public StudentRepositoryEventVisitor(DatabaseContext context)
    {
        _context = context;
    }

    public ValueTask VisitAsync(StudentTransferredEvent evt, CancellationToken cancellationToken)
    {
        EntityEntry<StudentModel> entry = RepositoryTools.GetEntry(
            _context,
            x => x.UserId.Equals(evt.Student.UserId),
            () => StudentMapper.MapFrom(evt.Student));

        entry.Entity.StudentGroupId = evt.Student.Group?.Id;
        entry.State = EntityState.Modified;

        return ValueTask.CompletedTask;
    }
}