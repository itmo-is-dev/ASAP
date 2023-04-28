namespace ITMO.Dev.ASAP.Google.Spreadsheets.Tools;

public class DriveParentProvider
{
    private readonly IList<string> _parents;

    public DriveParentProvider(string driveId)
    {
        _parents = new List<string> { driveId };
    }

    public IList<string> GetParents()
    {
        return _parents;
    }
}