using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;
using System.ComponentModel;
using System.Drawing;


namespace VinegarBot.DiscordBot.Commands
{
    public class UserCommands : CommandGroup
    {
        private readonly ILogger<UserCommands> _logger;
        private readonly FeedbackService _feedbackService;

        public UserCommands(ILogger<UserCommands> logger, FeedbackService feedbackService)
        {
            _logger = logger;
            _feedbackService = feedbackService;
        }

        [Command("ping")]
        [CommandType(ApplicationCommandType.ChatInput)]
        [Description("Testing ping.")]
        public async Task<IResult> PingAsync()
        {
            var reply = await _feedbackService.SendContextualEmbedAsync(new Embed(
                Title: "Test", 
                Description: "Pong!", 
                Colour: Color.PeachPuff),
            ct: CancellationToken);

            _logger.LogInformation($"Bot pinged successfully.");

            return reply.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(reply);
        }
    }
}
