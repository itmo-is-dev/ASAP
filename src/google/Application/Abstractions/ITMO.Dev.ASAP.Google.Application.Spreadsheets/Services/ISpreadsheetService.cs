using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Models;

namespace ITMO.Dev.ASAP.Google.Application.Spreadsheets.Services;

public interface ISpreadsheetService
{
    Task<SpreadsheetCreateResult> CreateSpreadsheetAsync(string title, CancellationToken token);
}