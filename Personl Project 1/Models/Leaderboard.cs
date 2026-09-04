using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PersonalProject1.Models
{
    public class Leaderboard
    {
        private readonly Dictionary<string, LeaderboardEntry> _entries = new();
        private readonly string _filePath;

        private Leaderboard(string filePath)
        {
            _filePath = filePath;
        }

        public static Leaderboard Load(string filePath)
        {
            var lb = new Leaderboard(filePath);
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var list = JsonSerializer.Deserialize<List<LeaderboardEntry>>(json);
                    if (list != null)
                    {
                        foreach (var e in list)
                            lb._entries[e.Name] = e;
                    }
                }
            }
            catch
            {
                // ignore load failures and start with empty leaderboard
            }

            return lb;
        }

        public void Save()
        {
            try
            {
                var list = _entries.Values.ToList();
                var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch
            {
                // best-effort save, ignore errors
            }
        }

        public void RecordRoll(string name, bool isWin)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "Player";

            if (!_entries.TryGetValue(name, out var entry))
            {
                entry = new LeaderboardEntry { Name = name };
                _entries[name] = entry;
            }

            entry.Rolls++;
            if (isWin) entry.Wins++; else entry.Losses++;
            Save();
        }

        public IEnumerable<LeaderboardEntry> Top(int count = 10)
        {
            return _entries.Values
                .OrderByDescending(e => e.Wins)
                .ThenByDescending(e => e.Rolls)
                .Take(count);
        }
    }
}
