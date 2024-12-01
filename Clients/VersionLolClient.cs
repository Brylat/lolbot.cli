using System.Net.Http.Json;
using LolBot.Cli.Clients.Interfaces;

namespace LolBot.Cli.Clients;

public class VersionLolClient(IHttpClientFactory httpClientFactory)
    : BaseLolClient(httpClientFactory), IVersionLolClient
{
    private const string VersionUrl = "api/versions.json";

    public async Task<string> GetLatestVersion()
    {
        var versions = await GetHttpClient().GetFromJsonAsync<List<string>>(VersionUrl);
        return versions!.First();
    }
}