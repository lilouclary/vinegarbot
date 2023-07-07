using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using VinegarBot.DiscordBot.Models;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Objects;

namespace VinegarBot.DiscordBot.Services
{
    public class UserLevelService : IUserLevelService
    {
        private readonly Dictionary<int, int> levelLegend = new Dictionary<int, int>()
        {
            {0, 0},
            {1, 100},
            {2, 255},
            {3, 475},
            {4, 770},
            {5, 1150},
            {6, 1625},
            {7, 2205},
            {8, 2900},
            {9, 3720},
            {10, 4675},
            {11, 5775},
            {12, 7030},
            {13, 8450},
            {14, 10045},
            {15, 11825},
            {16, 13800},
            {17, 15980},
            {18, 18375},
            {19, 20995},
            {20, 23850},
            {21, 26950},
            {22, 30305},
            {23, 33925},
            {24, 37820},
            {25, 42000},
            {26, 46475},
            {27, 51255},
            {28, 56350},
            {29, 61770},
            {30, 67525}
        };
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
                        // var result = records.Where(user => user.UserName == username);
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
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

        public int CheckUserLevelUp(IUser _user)
        {
            ClaryUser user = GetUser(_user);
            List<ClaryUser> users = new List<ClaryUser>();
            int currentLevel = user.UserLevel;

            try
            {
                if (user.UserPoints > levelLegend[user.UserLevel])
                {
                    foreach(KeyValuePair<int, int> entry in levelLegend)
                    {
                        var x = entry.Key;
                        var y = entry.Value;

                        if(user.UserPoints < levelLegend[entry.Key])
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
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

    }
}
