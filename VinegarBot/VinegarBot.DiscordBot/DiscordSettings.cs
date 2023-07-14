using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinegarBot.DiscordBot
{
    public class DiscordSettings
    {
        public string MetricsUri { get; set; } = "http://seq";
        public string MetricsToken { get; set; } = String.Empty;
        public string BotToken { get; set; }
    }
}
