using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Models;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Services;
using ITMO.Dev.ASAP.Google.Spreadsheets.Extensions;
using ITMO.Dev.ASAP.Google.Spreadsheets.Tools;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Google.Spreadsheets.Services;

public class SheetManagementService : ISheetService
{
    private const string UpdateAllFields = "*";
    private readonly ILogger<SheetManagementService> _logger;

    private readonly SheetsService _sheetsService;
    private readonly SheetTitleComparer _sheetTitleComparer;

    public SheetManagementService(
        SheetsService sheetsService,
        ILogger<SheetManagementService> logger)
    {
        _sheetsService = sheetsService;
        _sheetTitleComparer = new SheetTitleComparer();
        _logger = logger;
    }

    public async Task<SheetId> CreateOrClearSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token)
    {
        SheetId? sheetId = await GetSheetIdAsync(spreadsheetId, sheetTitle, token);

        if (sheetId is null)
        {
            sheetId = await CreateSheetAsync(spreadsheetId, sheetTitle, token);
            await SortSheetsAsync(spreadsheetId, token);
        }
        else
        {
            await ClearSheetAsync(spreadsheetId, sheetId.Value.Value, token);
        }

        return sheetId.Value;
    }

    public async Task<SheetId> CreateSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token)
    {
        var addSheetRequest = new Request
        {
            AddSheet = new AddSheetRequest { Properties = new SheetProperties { Title = sheetTitle } },
        };

        _logger.LogDebug("Create sheet with title {SheetTitle}", sheetTitle);

        BatchUpdateSpreadsheetResponse batchUpdateResponse = await _sheetsService
            .ExecuteBatchUpdateAsync(spreadsheetId, addSheetRequest, token);

        SheetProperties addedSheetProperties = batchUpdateResponse.Replies[0].AddSheet.Properties;

        return new SheetId(addedSheetProperties.SheetId!.Value);
    }

    public async Task<bool> SheetExistsAsync(string spreadsheetId, string sheetTitle, CancellationToken token)
    {
        SheetId? sheetId = await GetSheetIdAsync(spreadsheetId, sheetTitle, token);
        return sheetId is not null;
    }

    private async Task<SheetId?> GetSheetIdAsync(string spreadsheetId, string title, CancellationToken token)
    {
        IList<Sheet> sheets = await GetSheetsAsync(spreadsheetId, token);

        Sheet? sheet = sheets.FirstOrDefault(s => s.Properties.Title == title);
        int? id = sheet?.Properties.SheetId;

        return id is null ? null : new SheetId(id.Value);
    }

    private async Task SortSheetsAsync(string spreadsheetId, CancellationToken token)
    {
        IList<Sheet> sheets = await GetSheetsAsync(spreadsheetId, token);

        Request[] updateSheetIndexRequests = sheets
            .OrderBy(s => s.Properties.Title, _sheetTitleComparer)
            .Select((s, i) => (Sheet: s, NewIndex: i + 1))
            .Select(t =>
            {
                SheetProperties newProperties = t.Sheet.Properties;
                newProperties.Index = t.NewIndex;

                return new Request
                {
                    UpdateSheetProperties = new UpdateSheetPropertiesRequest
                    {
                        Properties = newProperties, Fields = UpdateAllFields,
                    },
                };
            })
            .ToArray();

        if (updateSheetIndexRequests.Length is not 0)
        {
            _logger.LogDebug("Reorder sheets in spreadsheet {SpreadsheetId}", spreadsheetId);

            await _sheetsService.ExecuteBatchUpdateAsync(spreadsheetId, updateSheetIndexRequests, token);
        }
    }

    private async Task ClearSheetAsync(string spreadsheetId, int sheetId, CancellationToken token)
    {
        var allFieldsGridRange = new GridRange { StartRowIndex = 0, StartColumnIndex = 0, SheetId = sheetId };

        var updateCellsRequest = new Request
        {
            UpdateCells = new UpdateCellsRequest { Range = allFieldsGridRange, Fields = UpdateAllFields },
        };

        var unmergeCellsRequest = new Request { UnmergeCells = new UnmergeCellsRequest { Range = allFieldsGridRange } };

        var deleteFreezeRequest = new Request
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Fields = "gridProperties.frozenColumnCount,gridProperties.frozenRowCount",
                Properties = new SheetProperties
                {
                    SheetId = sheetId,
                    GridProperties = new GridProperties
                    {
                        FrozenColumnCount = 0,
                        FrozenRowCount = 0,
                    },
                },
            },
        };

        var requests = new List<Request> { updateCellsRequest, unmergeCellsRequest, deleteFreezeRequest };

        _logger.LogDebug("Clear sheet with id {SheetId}", sheetId);
        await _sheetsService.ExecuteBatchUpdateAsync(spreadsheetId, requests, token);
    }

    private async Task<IList<Sheet>> GetSheetsAsync(string spreadsheetId, CancellationToken token)
    {
        _logger.LogDebug("Request spreadsheet with id {SpreadsheetId}", spreadsheetId);

        Spreadsheet spreadsheet = await _sheetsService.Spreadsheets
            .Get(spreadsheetId)
            .ExecuteAsync(token);

        return spreadsheet.Sheets;
    }
}