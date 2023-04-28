using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Models;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Services;
using ITMO.Dev.ASAP.Google.Common;
using ITMO.Dev.ASAP.Google.Spreadsheets.Extensions;
using ITMO.Dev.ASAP.Google.Spreadsheets.Tools;
using Microsoft.Extensions.Logging;
using File = Google.Apis.Drive.v3.Data.File;

namespace ITMO.Dev.ASAP.Google.Spreadsheets.Services;

public class SpreadsheetService : ISpreadsheetService
{
    private const string SpreadsheetType = "application/vnd.google-apps.spreadsheet";

    private const int DefaultSheetId = SheetConfigurations.Labs.Id;
    private const string DefaultSheetTitle = SheetConfigurations.Labs.Title;

    private const string UpdateTitle = "title";

    private static readonly Permission AnyoneViewerPermission = new Permission { Type = "anyone", Role = "reader" };

    private readonly DriveService _driveService;
    private readonly ILogger<SpreadsheetService> _logger;

    private readonly SheetsService _sheetsService;
    private readonly DriveParentProvider _tablesParentsProvider;

    public SpreadsheetService(
        SheetsService sheetsService,
        DriveService driveService,
        DriveParentProvider tablesParentsProvider,
        ILogger<SpreadsheetService> logger)
    {
        _sheetsService = sheetsService;
        _driveService = driveService;
        _tablesParentsProvider = tablesParentsProvider;
        _logger = logger;
    }

    public async Task<SpreadsheetCreateResult> CreateSpreadsheetAsync(string title, CancellationToken token)
    {
        var spreadsheetToCreate = new File
        {
            Parents = _tablesParentsProvider.GetParents(), MimeType = SpreadsheetType, Name = title,
        };

        _logger.LogDebug("Create file {Title} on Google drive", title);

        File spreadsheetFile = await _driveService.Files
            .Create(spreadsheetToCreate)
            .ExecuteAsync(token);

        string spreadsheetId = spreadsheetFile.Id;

        await ConfigureDefaultSheetAsync(spreadsheetId, token);

        _logger.LogDebug("Update permission of file: {Title}", title);

        await _driveService.Permissions
            .Create(AnyoneViewerPermission, spreadsheetId)
            .ExecuteAsync(token);

        return new SpreadsheetCreateResult(spreadsheetId);
    }

    private async Task ConfigureDefaultSheetAsync(string spreadsheetId, CancellationToken token)
    {
        var defaultSheetProperties = new SheetProperties { SheetId = DefaultSheetId, Title = DefaultSheetTitle };

        var updatePropertiesRequest = new Request
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = defaultSheetProperties, Fields = UpdateTitle,
            },
        };

        _logger.LogDebug("Configure default sheet for {SpreadsheetId}", spreadsheetId);

        await _sheetsService.ExecuteBatchUpdateAsync(spreadsheetId, updatePropertiesRequest, token);
    }
}