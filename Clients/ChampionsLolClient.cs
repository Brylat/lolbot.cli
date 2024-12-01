using System.Net.Http.Json;
using LolBot.Cli.Clients.Interfaces;
using LolBot.Cli.Models;

namespace LolBot.Cli.Clients;

public class ChampionsLolClient(IVersionLolClient versionLolClient, IHttpClientFactory httpClientFactory)
    : BaseLolClient(httpClientFactory), IChampionsLolClient
{
    private readonly string _championTemplateUrl = "cdn/{0}/data/en_US/champion.json";

    public async Task<ChampionsDataModel?> GetChampions()
    {
        var version = await versionLolClient.GetLatestVersion();
        var championUrl = string.Format(_championTemplateUrl, version);
        return await GetHttpClient().GetFromJsonAsync<ChampionsDataModel>(championUrl);
    }
}