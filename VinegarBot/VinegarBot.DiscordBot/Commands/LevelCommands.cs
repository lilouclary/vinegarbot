using CsvHelper;
using CsvHelper.Configuration;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using VinegarBot.DiscordBot.Models;

namespace VinegarBot.DiscordBot.Commands
{
    public class LevelCommands : CommandGroup
    {
        private readonly ILogger<UserCommands> logger;
        private readonly FeedbackService feedbackService;

        public LevelCommands(ILogger<UserCommands> logger, FeedbackService feedbackService)
        {
            this.logger = logger;
            this.feedbackService = feedbackService;
        }

        [Command("GetUserLevel")]
        [CommandType(ApplicationCommandType.User)]
        public async Task<IResult> GetUserLevelAsync(IUser user)
        {

            var username = user.Username;
            ClaryUser claryUser = GetUser(username);

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

        [Command("givepoints")]
        [CommandType(ApplicationCommandType.ChatInput)]
        [Description("Give points to specified user.")]
        public async Task<IResult> GivePointsAsync(IUser user, [Description("Enter an integer.")] int points)
        {
            ClaryUser claryUser = GetUser(user.Username);
            claryUser.UserPoints += points;

            ModifyUserPoints(user.Username, points);

            var reply = await feedbackService.SendContextualInfoAsync($"Gave {user.Username} {points} points.");

            return reply.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(reply);
        }

        private ClaryUser GetUser(string username)
        {
            ClaryUser user = null;
            bool userExists = CheckIfUserExists(username);

            try
            {
                if (userExists)
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false,
                    };
                    using (var reader = new StreamReader("UserInfo.csv"))
                    using (var csv = new CsvReader(reader, config))
                    {
                        var records = csv.GetRecords<ClaryUser>();
                        // var result = records.Where(user => user.UserName == username);
                        user = records.Where(user => user.UserName == username).FirstOrDefault();
                    }
                }
                else
                {
                    user = AddUser(username);
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message); 
            }
            

            return user;
        }

        private ClaryUser AddUser(string username)
        {
            ClaryUser newUser = new ClaryUser { UserName = username};

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                };

                using (var stream = File.Open("UserInfo.csv", FileMode.Append))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecord(newUser);
                    csv.NextRecord();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return newUser;
        }

        private void ModifyUserPoints(string username, int points)
        {
            List<ClaryUser> users = new List<ClaryUser>();

            // read file into enumerable and update points for user
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var reader = new StreamReader("UserInfo.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                List<ClaryUser> records = csv.GetRecords<ClaryUser>().ToList();
                
                foreach (var record in records)
                {
                    if(record.UserName == username)
                    {
                        record.UserPoints += points;
                    }
                }

                users = records;
            }

            // overwrite the old file with the new data
            using (var writer = new StreamWriter("UserInfo.csv", false))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(users);
            }
        }

        private bool CheckIfUserExists(string username)
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                };
                using (var reader = new StreamReader("UserInfo.csv"))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<ClaryUser>();
                    var result = records.Where(user => user.UserName == username);

                    if (result.Count() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }
    }
}
