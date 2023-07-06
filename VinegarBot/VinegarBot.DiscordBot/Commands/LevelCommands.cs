using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Extensions.Embeds;
using Remora.Discord.Pagination;
using Remora.Discord.Pagination.Extensions;
using Remora.Results;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using VinegarBot.DiscordBot.Models;
using VinegarBot.DiscordBot.Services;

namespace VinegarBot.DiscordBot.Commands
{
    public class LevelCommands : CommandGroup
    {
        private readonly ILogger<UserCommands> logger;
        private readonly FeedbackService feedbackService;
        private readonly IUserLevelService userLevelService;

        public LevelCommands(ILogger<UserCommands> logger, FeedbackService feedbackService, IUserLevelService userLevelService)
        {
            this.logger = logger;
            this.feedbackService = feedbackService;
            this.userLevelService = userLevelService;
        }

        [Command("checklevel")]
        [CommandType(ApplicationCommandType.User)]
        [DiscordDefaultMemberPermissions(DiscordPermission.SendMessages)]
        public async Task<IResult> GetUserLevelAsync(IUser user)
        {
            var username = user.Username;
            ClaryUser claryUser = userLevelService.GetUser(user);

            string responseText = $"{username} has {claryUser.UserPoints} points and is level {claryUser.UserLevel}.";

            var reply = await feedbackService.SendContextualEmbedAsync(new Embed(
                Title: "Test",
                Description: responseText,
                Colour: Color.PeachPuff),
                ct: CancellationToken);

            return reply.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(reply);
        }

        [Command("updatepoints")]
        [CommandType(ApplicationCommandType.ChatInput)]
        [Description("Give points to specified user.")]
        //[DiscordDefaultMemberPermissions(DiscordPermission.ModerateMembers)]
        public async Task<IResult> GivePointsAsync(
            IUser user, 
            [AutocompleteProvider("autocomplete::leveloptions"), Description("What kind of modification? (Add or Subtract)")] string action, 
            [Description("Enter an integer.")] int points)
        {
            var replyText = $"Added {points} points to {user.Username}.";

            if (action == "subtract")
            {
                points = points * -1;
                replyText = $"Subtracted {Math.Abs(points)} points from {user.Username}.";
            }

            ClaryUser claryUser = userLevelService.GetUser(user);
            claryUser.UserPoints += points;

            userLevelService.ModifyUserPoints(user, points);

            var reply = await feedbackService.SendContextualInfoAsync(replyText);

            return reply.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(reply);
        }

        [Command("checkleaderboard")]
        [CommandType(ApplicationCommandType.ChatInput)]
        [Description("Check the Clary Rewards Program leaderboard.")]
        public async Task<IResult> CheckLeaderboardAsync()
        {
            List<ClaryUser> users = userLevelService.GetUsers();
            var fields = new List<EmbedField>();
            int ranking = 1;

            foreach (var user in users)
            {
                fields.Add(new EmbedField("", $"{ranking}. {user.UserName}  {user.UserPoints}xp  lvl{user.UserLevel}"));
                ranking++;
            }

            //var pages = PaginatedEmbedFactory.SimpleFieldsFromCollection(users, user => user.UserName, user => user.UserPoints.ToString());

            var reply = await feedbackService.SendContextualEmbedAsync(new Embed(
                Title: "Clary Reward's Program Leaderboard",
                Fields: fields
                ));

            return reply.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(reply);
        }
    }
}
