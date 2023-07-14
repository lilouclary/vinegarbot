using Remora.Rest.Core;

namespace VinegarBot.DiscordBot.Models
{
    public class ClaryUser
    {
        public required string UserName { get; set; }
        public required Snowflake UserID { get; set; }
        public int UserPoints { get; set; } = 0;
        public int UserLevel { get; set; } = 0;
        public DateTimeOffset LastMessageDateTime { get; set; } = DateTimeOffset.MinValue;
    }
}
