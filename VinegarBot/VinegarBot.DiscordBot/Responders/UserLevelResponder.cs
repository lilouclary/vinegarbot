using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinegarBot.DiscordBot.Models;
using VinegarBot.DiscordBot.Services;

namespace VinegarBot.DiscordBot.Responders
{
    public class UserLevelResponder : IResponder<IMessageCreate>
    {
        private readonly IUserLevelService userLevelService;
        public UserLevelResponder(IUserLevelService userLevelService) 
        {
            this.userLevelService = userLevelService;
        }
        public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            if ((gatewayEvent.Author.IsBot.TryGet(out var isBot) && isBot) ||
                (gatewayEvent.Author.IsSystem.TryGet(out var isSystem) && isSystem) ||
                (!userLevelService.CheckMessageInterval(gatewayEvent.Author, gatewayEvent.Timestamp)))
            {
                return Result.FromSuccess();
            }

            userLevelService.ModifyUserPoints(gatewayEvent.Author, userLevelService.DeterminePostXP());

            return (Result.FromSuccess());
        }
    }
}
