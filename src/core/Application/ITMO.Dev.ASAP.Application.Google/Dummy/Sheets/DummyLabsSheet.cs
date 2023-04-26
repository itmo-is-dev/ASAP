using ITMO.Dev.ASAP.Application.Abstractions.Google.Sheets;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;

namespace ITMO.Dev.ASAP.Application.Google.Dummy.Sheets;

public class DummyLabsSheet : ISheet<SubjectCoursePointsDto>
{
    public Task UpdateAsync(string spreadsheetId, SubjectCoursePointsDto model, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}