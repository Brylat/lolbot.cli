using System.Text;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LolBot.Cli.Clients;
using LolBot.Cli.Clients.Interfaces;
using LolBot.Cli.Services;
using LolBot.Cli.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

var services = new ServiceCollection();
services.AddTransient<IVersionLolClient, VersionLolClient>();
services.AddTransient<IChampionsLolClient, ChampionsLolClient>();
services.AddTransient<ITeamsService, TeamsService>();
services.AddHttpClient();
var serviceProvider = services.BuildServiceProvider() ?? throw new NullReferenceException("Null Service Provider");
var discordClient = new DiscordSocketClient(new()
{
    UseInteractionSnowflakeDate = false
});
StartDiscordBot().GetAwaiter().GetResult();

async Task StartDiscordBot()
{
    var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        throw new ArgumentNullException(nameof(token));
    }

    await discordClient.LoginAsync(TokenType.Bot, token);
    discordClient.Ready += ClientReady;
    discordClient.SlashCommandExecuted += SlashCommandHandler;
    discordClient.ModalSubmitted += ModalCommandHandler;
    await discordClient.StartAsync();
    await Task.Delay(-1);
}

async Task ClientReady()
{
    List<ApplicationCommandProperties> applicationCommandProperties = new();
    try
    {
        var globalDrawAramChampion = new SlashCommandBuilder();
        globalDrawAramChampion
            .WithName("draw-aram-champions")
            .WithDescription("Return random list of champions for two teams.");
        applicationCommandProperties.Add(globalDrawAramChampion.Build());

        await discordClient.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
        Console.WriteLine("Initialized");
    }
    catch (HttpException exception)
    {
        var json = JsonConvert.SerializeObject(exception, Formatting.Indented);
        Console.WriteLine(json);
    }
}

async Task ModalCommandHandler(SocketModal modal)
{
    List<SocketMessageComponentData> components =
        modal.Data.Components.ToList();
    var players = components
        .First(x => x.CustomId == "players").Value;
    var playersList = players.Split('\n').Where(x => !string.IsNullOrEmpty(x));
    var random = new Random();
    var shuffled = playersList.OrderBy(x => random.Next()).ToList();
    int mid = shuffled.Count / 2;
    await modal.RespondAsync(GenerateTeamResponse(shuffled.Take(mid).ToList(), shuffled.Skip(mid).ToList()));
}

async Task SlashCommandHandler(SocketSlashCommand command)
{
    try
    {
        switch (command.CommandName)
        {
            case "draw-aram-champions":
            {
                var teamService = serviceProvider!.GetService<ITeamsService>()!;
                var teamsResponse = await teamService.GetTeams();
                var response = GenerateTeamResponse(teamsResponse.FirstTeam, teamsResponse.SecondTeam);
                await command.RespondAsync(response);
                break;
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    
}

string GenerateTeamResponse(List<string> firstTeam, List<string> secondTeam)
{
    var stringBuilder = new StringBuilder();
    stringBuilder.Append($"Team 1:{Environment.NewLine}");
    stringBuilder.Append(string.Join(Environment.NewLine, firstTeam.Select(x => $"- {x}")));
    var s = Format.Code(stringBuilder.ToString());
    stringBuilder.Clear();
    stringBuilder.Append($"Team 2:{Environment.NewLine}");
    stringBuilder.Append(string.Join(Environment.NewLine, secondTeam.Select(x => $"- {x}")));
    s += Format.Code(stringBuilder.ToString());
    return s;
}