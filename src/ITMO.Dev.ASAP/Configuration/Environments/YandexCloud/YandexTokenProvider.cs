using ITMO.Dev.ASAP.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace ITMO.Dev.ASAP;

internal static class YandexTokenProvider
{
    internal static async Task<string> GetToken()
    {
        const string baseUrl = "http://169.254.169.254/";
        const string requestUri = "computeMetadata/v1/instance/service-accounts/default/token";

        using var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri)
        {
            Headers =
            {
                { "Metadata-Flavor", "Google" },
            },
        };

        HttpResponseMessage resp = await httpClient.SendAsync(request);

        if (resp.StatusCode is not HttpStatusCode.OK)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Unable to receive IAM service account token from Yandex Compute Cloud.");
            stringBuilder.AppendLine($"HTTP Status code: {resp.StatusCode:D)}");
            string body = await resp.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body) is false)
            {
                stringBuilder.AppendLine("HTTP Response:");
                stringBuilder.AppendLine(body);
            }

            throw new StartupException(stringBuilder.ToString());
        }

        string content = await resp.Content.ReadAsStringAsync();
        JObject? jsonContent = JsonConvert.DeserializeObject<JObject>(content);

        if (jsonContent is null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Unable to parse IAM service account token from Yandex Compute Cloud.");
            stringBuilder.AppendLine("Cannot parse JSON. Original response:");
            stringBuilder.AppendLine(content);

            throw new StartupException(stringBuilder.ToString());
        }

        if (jsonContent.TryGetValue("access_token", StringComparison.Ordinal, out JToken? accessToken))
        {
            return accessToken.ToString();
        }

        var parseStringBuilder = new StringBuilder();
        parseStringBuilder.AppendLine("Unable to parse IAM service account token from Yandex Compute Cloud.");
        parseStringBuilder.AppendLine("Cannot find access token in original response:");
        parseStringBuilder.AppendLine(content);

        throw new StartupException(parseStringBuilder.ToString());
    }
}