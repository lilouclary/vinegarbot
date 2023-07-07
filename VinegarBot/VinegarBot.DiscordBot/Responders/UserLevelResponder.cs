using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway.Responders;
using Remora.Results;
using System.Drawing;
using VinegarBot.DiscordBot.Services;

namespace VinegarBot.DiscordBot.Responders
{
    public class UserLevelResponder : IResponder<IMessageCreate>
    {
        private readonly IUserLevelService _userLevelService;
        private readonly IDiscordRestChannelAPI _channelAPI;
        private readonly List<int> levelMilestones = new() { 3, 10, 15, 20 };

        public UserLevelResponder(IDiscordRestChannelAPI channelAPI, IUserLevelService userLevelService) 
        {
            _channelAPI = channelAPI;
            _userLevelService = userLevelService;
        }
        public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            if ((gatewayEvent.Author.IsBot.TryGet(out var isBot) && isBot) ||
                (gatewayEvent.Author.IsSystem.TryGet(out var isSystem) && isSystem) ||
                (!_userLevelService.CheckMessageInterval(gatewayEvent.Author, gatewayEvent.Timestamp)))
            {
                return Result.FromSuccess();
            }

            _userLevelService.ModifyUserPoints(gatewayEvent.Author, _userLevelService.DeterminePostXP());
            int userLevel = _userLevelService.CheckUserLevelUp(gatewayEvent.Author);

            if (levelMilestones.Contains(userLevel))
            {
                var embed = new Embed(
                    Description: $"Congratulations {gatewayEvent.Author.Username}, you have leveled up to level {userLevel}! Please check the pinned post for your new rewards!", 
                    Colour: Color.PeachPuff);

                return (Result) await _channelAPI.CreateMessageAsync
                (
                    gatewayEvent.ChannelID,
                    embeds: new[] { embed },
                    ct: ct
                );
            }
            else
            {
                return (Result.FromSuccess());
            }
        }
    }
}
