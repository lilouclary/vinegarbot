﻿using Remora.Discord.API.Abstractions.Objects;
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
        public int CheckUserLevelUp(IUser _user);
    }
}
