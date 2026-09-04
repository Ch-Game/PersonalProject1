using System;

namespace PersonalProject1.Models
{
    public class LeaderboardEntry
    {
        public string Name { get; set; } = string.Empty;
        public int Rolls { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}
