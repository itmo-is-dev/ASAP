using System.Text;
using Index = FluentSpreadsheets.Index;

namespace ITMO.Dev.ASAP.Google.Application.Extensions;

internal static class IndexExtensions
{
    public static string ToGoogleSheetsIndex(this Index index)
    {
        return $"{GetColumnIndex(index.Column)}{index.Row}";
    }

    public static Index WithRowShift(this Index index, int shift)
    {
        return index with { Row = index.Row + shift };
    }

    public static Index WithColumnShift(this Index index, int shift)
    {
        return index with { Column = index.Column + shift };
    }

    private static string GetColumnIndex(int value)
    {
        if (value is 0)
            return string.Empty;

        var stringBuilder = new StringBuilder();

        while (value > 0)
        {
            int num = (value - 1) % 26;
            stringBuilder.Append((char)(65 + num));
            value = (value - num) / 26;
        }

        return stringBuilder.ToString();
    }
}