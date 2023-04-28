using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Google.Application.Abstractions;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Models;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Services;

namespace ITMO.Dev.ASAP.Google.Application.Sheets;

public class QueueSheet : ISheet<SubmissionsQueueDto>
{
    private readonly ITable<SubmissionsQueueDto> _queueTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;
    private readonly ISheetService _sheetService;

    public QueueSheet(
        ISheetService sheetService,
        ITable<SubmissionsQueueDto> queueTable,
        IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _sheetService = sheetService;
        _queueTable = queueTable;
        _renderer = renderer;
    }

    public async Task UpdateAsync(string spreadsheetId, SubmissionsQueueDto model, CancellationToken token)
    {
        string title = model.GroupName;

        SheetId sheetId = await _sheetService.CreateOrClearSheetAsync(spreadsheetId, title, token);

        IComponent sheetData = _queueTable.Render(model);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, sheetId.Value, title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }
}