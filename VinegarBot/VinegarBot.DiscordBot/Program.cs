using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Hosting.Extensions;
using Remora.Rest.Core;
using Serilog;
using VinegarBot.DiscordBot;
using VinegarBot.DiscordBot.Autocomplete;
using VinegarBot.DiscordBot.Commands;
using VinegarBot.DiscordBot.Services;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", true)
    .AddEnvironmentVariables()
    .Build();

var settings = configuration
    .GetSection(nameof(DiscordSettings))
    .Get<DiscordSettings>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Seq(
        serverUrl: settings.MetricsUri,
        apiKey: settings.MetricsToken)
    // .ReadFrom.Configuration(configuration)
    .CreateLogger();

var responderTypes = typeof(Program).Assembly
    .GetExportedTypes()
    .Where(t => t.IsResponder());

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog(Log.Logger)
    .AddDiscordService(_ => settings.BotToken)
    .ConfigureServices(services =>
    {
        services
            .AddDiscordCommands(true)
            .AddCommandTree()
                .WithCommandGroup<UserCommands>()
                .WithCommandGroup<LevelCommands>()
                .Finish()
            .AddAutocompleteProvider<LevelsOptionsAutocompleteProvider>()
            .AddTransient<IUserLevelService, UserLevelService>();

        foreach (var type in responderTypes)
        {
            services.AddResponder(type);
        }
    })
    .Build();
await host.Services.GetService<SlashService>().UpdateSlashCommandsAsync(new Snowflake(969594527151177798));
host.Run();
