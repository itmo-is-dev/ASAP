namespace ITMO.Dev.ASAP.Google.Spreadsheets.Tools;

public class GoogleIntegrationConfiguration
{
    public string ClientSecrets { get; set; } = string.Empty;

    public string GoogleDriveId { get; set; } = string.Empty;

    public bool EnableGoogleIntegration { get; set; }
}