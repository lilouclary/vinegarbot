using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using VinegarBot.DiscordBot.Models;
using Remora.Discord.API.Abstractions.Objects;
using VinegarBot.DiscordBot.Config;

namespace VinegarBot.DiscordBot.Services
{
    public class UserLevelService : IUserLevelService
    {
        ILogger<UserLevelService> _logger;

        public UserLevelService(ILogger<UserLevelService> logger)
        {
            _logger = logger;
        }

        public ClaryUser GetUser(IUser _user)
        {
            ClaryUser claryUser = null;
            bool userExists = CheckIfUserExists(_user);

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
                        claryUser = records.Where(user => user.UserName == _user.Username).FirstOrDefault();
                    }
                }
                else
                {
                    claryUser = AddUser(_user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }


            return claryUser;
        }

        public ClaryUser AddUser(IUser _user)
        {
            ClaryUser newUser = new ClaryUser { UserName = _user.Username, UserID = _user.ID};

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
                _logger.LogError(ex.Message, ex);
            }

            return newUser;
        }

        public bool CheckIfUserExists(IUser _user)
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
                    var result = records.Where(user => user.UserName == _user.Username);

                    return result.Any();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex); 
            }

            return false;
        }

        public List<ClaryUser> GetUsers()
        {
            List<ClaryUser> users = new List<ClaryUser>();

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
                    users =  records.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return users;
        }

        public void ModifyUserPoints(IUser _user, int points)
        {
            List<ClaryUser> users = new List<ClaryUser>();

            if (!CheckIfUserExists(_user))
            {
                AddUser(_user);
            }

            try
            {
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
                        if (record.UserName == _user.Username)
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public int DeterminePostXP()
        {
            Random random = new Random();
            int XP = random.Next(15, 25);

            return XP;
        }

        public bool CheckMessageInterval(IUser _user, DateTimeOffset eventDateTime)
        {
            ClaryUser user = GetUser(_user);
            List<ClaryUser> users = new List<ClaryUser>();

            TimeSpan offset = eventDateTime - user.LastMessageDateTime;

            if(offset.TotalSeconds < 60)
            {
                return false;
            }
            else
            {
                // update the timestamp for the user if they can be awarded points
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
                        if (record.UserName == _user.Username)
                        {
                            record.LastMessageDateTime = eventDateTime;
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

                return true;
            }
        }

        // Return 0 if level up has not occured
        // Return actual level if level has changed
        public int CheckUserLevelUp(IUser _user)
        {
            ClaryUser user = GetUser(_user);
            List<ClaryUser> users = new List<ClaryUser>();
            int currentLevel = user.UserLevel;

            try
            {
                if (user.UserPoints > LevelXPDictionary.levelLegend[user.UserLevel + 1])
                {
                    foreach(KeyValuePair<int, int> entry in LevelXPDictionary.levelLegend)
                    {
                        var x = entry.Key;
                        var y = entry.Value;

                        if(user.UserPoints < LevelXPDictionary.levelLegend[entry.Key])
                        {
                            currentLevel = entry.Key - 1;
                            break;
                        }
                    }

                    // update level for the current user if they have leveled up
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
                            if (record.UserName == _user.Username)
                            {
                                record.UserLevel = currentLevel;
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

                    return currentLevel;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return 0;
            }
        }

    }
}
