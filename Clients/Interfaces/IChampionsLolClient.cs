using LolBot.Cli.Models;

namespace LolBot.Cli.Clients.Interfaces;

public interface IChampionsLolClient
{
    Task<ChampionsDataModel?> GetChampions();
}