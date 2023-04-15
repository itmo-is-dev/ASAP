namespace ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;

public readonly record struct ExceptionMessage(string? Title, string? Message, Exception Exception);