namespace ITMO.Dev.ASAP.WebUI.Abstractions.Models;

public record ExceptionDisplayConfiguration(TimeSpan PopupLifetime, bool ShowExceptionDetails = true);