using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Google.Application.Abstractions;
using ITMO.Dev.ASAP.Google.Application.Abstractions.Models;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Models;
using ITMO.Dev.ASAP.Google.Application.Spreadsheets.Services;
using ITMO.Dev.ASAP.Google.Common;

namespace ITMO.Dev.ASAP.Google.Application.Sheets;

public class LabsSheet : ISheet<SubjectCoursePointsDto>
{
    public const string Title = SheetConfigurations.Labs.Title;

    private readonly ISheet<CourseStudentsDto> _pointsSheet;
    private readonly ITable<SubjectCoursePointsDto> _pointsTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;
    private readonly ISheetService _sheetService;

    public LabsSheet(
        ISheetService sheetService,
        ITable<SubjectCoursePointsDto> pointsTable,
        IComponentRenderer<GoogleSheetRenderCommand> renderer,
        ISheet<CourseStudentsDto> pointsSheet)
    {
        _sheetService = sheetService;
        _pointsTable = pointsTable;
        _renderer = renderer;
        _pointsSheet = pointsSheet;
    }

    public async Task UpdateAsync(string spreadsheetId, SubjectCoursePointsDto model, CancellationToken token)
    {
        SheetId sheetId = await _sheetService.CreateOrClearSheetAsync(spreadsheetId, Title, token);

        IComponent sheetData = _pointsTable.Render(model);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, sheetId.Value, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);

        bool pointsSheetExists = await _sheetService.SheetExistsAsync(spreadsheetId, PointsSheet.Title, token);

        if (pointsSheetExists is false)
        {
            var courseStudents = new CourseStudentsDto(model.StudentsPoints);
            await _pointsSheet.UpdateAsync(spreadsheetId, courseStudents, token);
        }
    }
}