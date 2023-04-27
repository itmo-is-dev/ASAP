using ITMO.Dev.ASAP.Application.Abstractions.Google;

namespace ITMO.Dev.ASAP.Application.Google.Dummy.Services;

public class DummySubjectCourseTableService : ISubjectCourseTableService
{
    public Task<string> GetSubjectCourseTableId(Guid subjectCourseId, CancellationToken cancellationToken)
    {
        return Task.FromResult(string.Empty);
    }
}