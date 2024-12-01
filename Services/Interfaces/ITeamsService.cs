namespace LolBot.Cli.Services.Interfaces;

public interface ITeamsService
{
    Task<(List<string> FirstTeam, List<string> SecondTeam)> GetTeams();
}