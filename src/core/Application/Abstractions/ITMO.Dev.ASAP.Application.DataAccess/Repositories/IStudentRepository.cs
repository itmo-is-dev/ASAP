using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Students;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface IStudentRepository
{
    IAsyncEnumerable<Student> QueryAsync(StudentQuery query, CancellationToken cancellationToken);

    ValueTask ApplyAsync(IStudentEvent evt, CancellationToken cancellationToken);

    void Add(Student student);

    void Update(Student student);
}