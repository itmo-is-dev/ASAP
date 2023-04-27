using ITMO.Dev.ASAP.Application.Abstractions.Google.Sheets;
using ITMO.Dev.ASAP.Application.Dto.Tables;

namespace ITMO.Dev.ASAP.Application.Google.Dummy.Sheets;

public class DummyQueueSheet : ISheet<SubmissionsQueueDto>
{
    public Task UpdateAsync(string spreadsheetId, SubmissionsQueueDto model, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}