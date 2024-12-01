namespace LolBot.Cli.Clients.Interfaces;

public interface IVersionLolClient
{
    Task<string> GetLatestVersion();
}