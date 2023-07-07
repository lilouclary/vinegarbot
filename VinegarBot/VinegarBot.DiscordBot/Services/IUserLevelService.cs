using Remora.Discord.API.Abstractions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinegarBot.DiscordBot.Models;

namespace VinegarBot.DiscordBot.Services
{
    public interface IUserLevelService
    {
        public ClaryUser GetUser(IUser _user);
        public ClaryUser AddUser(IUser _user);
        public bool CheckIfUserExists(IUser _user);
        public List<ClaryUser> GetUsers();
        public void ModifyUserPoints(IUser _user, int points);
        public int DeterminePostXP();
        public bool CheckMessageInterval(IUser _user, DateTimeOffset eventDateTime);
    }
}
