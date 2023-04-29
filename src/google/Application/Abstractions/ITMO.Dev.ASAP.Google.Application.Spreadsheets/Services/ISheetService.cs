using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Models;

namespace ITMO.Dev.ASAP.Google.Application.Spreadsheets.Services;

public interface ISheetService
{
    Task<SheetId> CreateOrClearSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token);

    Task<SheetId> CreateSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token);

    Task<bool> SheetExistsAsync(string spreadsheetId, string sheetTitle, CancellationToken token);
}