using FuzzySharp;
using Humanizer;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Autocomplete;

namespace VinegarBot.DiscordBot.Autocomplete
{
    public class LevelsOptionsAutocompleteProvider : IAutocompleteProvider
    {
        private readonly IReadOnlySet<string> _dictionary = new SortedSet<string>
        {
            "add",
            "subtract"
        };

        public string Identity => "autocomplete::leveloptions";

        public ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync
        (
            IReadOnlyList<IApplicationCommandInteractionDataOption> options,
            string userInput,
            CancellationToken ct = default
        )
        {
            return new ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>
            (
                _dictionary
                    .OrderByDescending(n => Fuzz.Ratio(userInput, n))
                    .Take(2)
                    .Select(n => new ApplicationCommandOptionChoice(n.Humanize().Transform(To.TitleCase), n))
                    .ToList()
            );
        }
    }
}
