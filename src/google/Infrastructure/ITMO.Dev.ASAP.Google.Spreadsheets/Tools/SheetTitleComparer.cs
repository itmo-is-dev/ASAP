using ITMO.Dev.ASAP.Google.Spreadsheets.Extensions;

namespace ITMO.Dev.ASAP.Google.Spreadsheets.Tools;

public class SheetTitleComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y)
            return 0;

        if (x is null)
            return -1;

        if (y is null)
            return 1;

        return (x.HasCyrillic(), y.HasCyrillic()) switch
        {
            (true, false) => -1,
            (false, true) => 1,
            _ => string.Compare(x, y, StringComparison.InvariantCulture),
        };
    }
}