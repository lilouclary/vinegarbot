namespace VinegarBot.DiscordBot.Config
{
    public static class LevelXPDictionary
    {
        public static readonly Dictionary<int, int> levelLegend = new Dictionary<int, int>()
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

        public static readonly Dictionary<string, int> roleMilestones = new Dictionary<string, int>
        {
            { "Sprout", 3},
            { "Return Customer", 10 },
            { "Regular", 15},
            { "VIP", 20 }
        };
    }
}
