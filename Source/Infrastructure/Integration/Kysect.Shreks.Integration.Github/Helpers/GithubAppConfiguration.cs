﻿namespace Kysect.Shreks.Integration.Github.Helpers;

public class GithubAppConfiguration : IShreksConfiguration
{
    public string? PrivateKey { get; init; }
    public int AppIntegrationId { get; init; }
    public int JwtExpirationSeconds { get; init; }
    public string? GithubAppSecret { get; init; }
    public string? ServiceOrganizationName { get; init; }

    public void Verify()
    {
        ArgumentNullException.ThrowIfNull(GithubAppSecret, nameof(GithubAppSecret));
        ArgumentNullException.ThrowIfNull(PrivateKey, nameof(PrivateKey));


        if (JwtExpirationSeconds <= 0)
        {
            throw new ArgumentException(
                $"Expiration in {nameof(GithubIntegrationConfiguration)} must be greater than 0");
        }

        if (AppIntegrationId <= 0)
        {
            throw new ArgumentException(
                $"AppIntegrationId in {nameof(GithubIntegrationConfiguration)} must be greater than 0");
        }
    }
}