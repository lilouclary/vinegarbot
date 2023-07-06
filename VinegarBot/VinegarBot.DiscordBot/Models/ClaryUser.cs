using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinegarBot.DiscordBot.Models
{
    public class ClaryUser
    {
        public required string UserName { get; set; }
        public int UserPoints { get; set; } = 0;
        public int UserLevel { get; set; } = 0;
    }
}
