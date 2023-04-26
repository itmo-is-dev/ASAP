using ITMO.Dev.ASAP.Application.Abstractions.Google.Sheets;
using ITMO.Dev.ASAP.Integration.Google.Models;

namespace ITMO.Dev.ASAP.Application.Google.Dummy.Sheets;

public class DummyPointsSheet : ISheet<CourseStudentsDto>
{
    public Task UpdateAsync(string spreadsheetId, CourseStudentsDto model, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}