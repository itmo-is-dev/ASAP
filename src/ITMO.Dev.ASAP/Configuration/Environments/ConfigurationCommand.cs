namespace ITMO.Dev.ASAP.Configuration.Environments;

public record ConfigurationCommand(string Environment, WebApplicationBuilder ApplicationBuilder);