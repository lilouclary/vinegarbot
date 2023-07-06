using Remora.Commands.Attributes;
using Remora.Commands.Groups;
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
        private readonly ILogger<UserCommands> logger;
        private readonly FeedbackService feedbackService;

        public UserCommands(ILogger<UserCommands> logger, FeedbackService feedbackService)
        {
            this.logger = logger;
            this.feedbackService = feedbackService;
        }

        [Command("ping")]
        [CommandType(ApplicationCommandType.ChatInput)]
        [Description("Testing ping.")]
        public async Task<IResult> PingAsync()
        {
            var reply = await feedbackService.SendContextualEmbedAsync(new Embed(
                Title: "Test", 
                Description: "Pong!", 
                Colour: Color.PeachPuff),
                ct: CancellationToken);

            return reply.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(reply);
        }
    }
}
