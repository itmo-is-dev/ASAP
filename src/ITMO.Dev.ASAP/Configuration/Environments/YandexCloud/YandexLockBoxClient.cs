using ITMO.Dev.ASAP.Configuration;
using ITMO.Dev.ASAP.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ITMO.Dev.ASAP;

internal class YandexLockBoxClient
{
    private readonly string _token;

    public YandexLockBoxClient(string token)
    {
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));

        _token = token;
    }

    internal async Task<LockBoxEntry[]> GetEntries(string secretId)
    {
        ArgumentException.ThrowIfNullOrEmpty(secretId, nameof(secretId));

        const string baseUrl = "https://payload.lockbox.api.cloud.yandex.net/";
        string requestUri = $"/lockbox/v1/secrets/{secretId}/payload";

        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", _token),
            },
        };

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        HttpResponseMessage resp = await httpClient.SendAsync(request);

        if (resp.StatusCode is not HttpStatusCode.OK)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Unable to receive secrets from Yandex LockBox.");
            stringBuilder.AppendLine($"HTTP Status code: {resp.StatusCode:D)}");
            string body = await resp.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(body))
            {
                stringBuilder.AppendLine("HTTP Response:");
                stringBuilder.AppendLine(body);
            }

            throw new StartupException(stringBuilder.ToString());
        }

        string respBody = await resp.Content.ReadAsStringAsync();
        try
        {
            LockBoxEntry[]? entries = JsonConvert.DeserializeObject<JObject>(respBody)?
                .GetValue("entries", StringComparison.Ordinal)?
                .ToObject<LockBoxEntry[]>();

            if (entries?.Any() is not true)
            {
                throw new StartupException("Secrets cannot be null or empty");
            }

            return entries;
        }
        catch
        {
            throw new StartupException("Yandex LockBox have not returned any secrets");
        }
    }
}