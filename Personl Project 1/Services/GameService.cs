using System;
using System.IO;
using System.Linq;
using PersonalProject1.Models;
using PersonalProject1.Utilities;

namespace PersonalProject1.Services
{
    public class GameService
    {
        private decimal PocketMoney = 1000m;
        private decimal playCost = 0.50m;
        private decimal rerollCost = 0.50m;
        private decimal winAmount = 100m;
        private int maxRoll = 100;
        private int luck = 0;

        private const int SpecialNumber = 50;
        private const string SpecialMessage = "You rolled 50 — A Indian scammer stole your money!";
        private const string SpecialLoseHalfMessage = "Special effect: you lose half your money";

        private const int RangeLoseMin = 20;
        private const int RangeLoseMax = 30;
        private const decimal RangeLoseAmount = 5m;
        private const string RangeLoseMessage = "You rolled between 20 and 30 — Your mom stole $5.00 from you.";

        private const int RangeLose2Min = 75;
        private const int RangeLose2Max = 85;
        private const decimal RangeLose2Amount = 10m;
        private const string RangeLose2Message = "You rolled between 75 and 85 — Mr. Preston asked for your Tutition Money You lose $10.00.";

        private const int RangeWinMin = 51;
        private const int RangeWinMax = 55;
        private const decimal RangeWinAmount = 15m;

        private const int RangeWin2Min = 93;
        private const int RangeWin2Max = 90; // original had reversed bounds; keep as-is
        private const decimal RangeWin2Amount = 25m;

        private readonly Leaderboard _leaderboard;
        private readonly string _playerName;

        public GameService(string playerName, Leaderboard leaderboard)
        {
            _playerName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
            _leaderboard = leaderboard ?? throw new ArgumentNullException(nameof(leaderboard));
        }

        private void KickIfBroke()
        {
            if (PocketMoney <= 0m)
            {
                Console.Clear();
                Console.WriteLine("You have been kicked out of the game called LIFE.");
                Environment.Exit(0);
            }
        }

        // Clear the console safely. Falls back to printing new lines when Clear() is not available
        private void SafeClear()
        {
            if (Console.IsOutputRedirected)
            {
                for (int i = 0; i < 50; i++) Console.WriteLine();
                return;
            }

            try
            {
                Console.Clear();
            }
            catch
            {
                try
                {
                    for (int i = 0; i < Console.WindowHeight; i++) Console.WriteLine();
                    Console.SetCursorPosition(0, 0);
                }
                catch
                {
                    // last resort: do nothing
                }
            }
        }

        public decimal Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\nYou have ${PocketMoney:F2} in your pocket.");
                KickIfBroke();
                Console.WriteLine("Choose: (P)lay, (U)pgrades, (L)eaderboard, (R)eset leaderboard, (C)lear, (Q)uit");
                string? choice = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(choice))
                    continue;

                choice = choice.Trim().ToUpperInvariant();
                if (choice == "Q")
                    break;

                if (choice == "U")
                {
                    ShowUpgradesMenu();
                    continue;
                }

                if (choice == "C")
                {
                    SafeClear();
                    continue;
                }

                if (choice == "L")
                {
                    Console.Clear();
                    Console.WriteLine("\nLeaderboard (top players by wins):");
                    foreach (var e in _leaderboard.Top(20))
                    {
                        Console.WriteLine($"{e.Name} - Rolls: {e.Rolls}, Wins: {e.Wins}, Losses: {e.Losses}");
                    }
                    continue;
                }

                if (choice == "R")
                {
                    Console.Clear();
                    Console.Write("Are you sure you want to reset the leaderboard? This cannot be undone. (Y/N): ");
                    string? confirm = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(confirm) && confirm.Trim().ToUpperInvariant() == "Y")
                    {
                        try
                        {
                            if (File.Exists("leaderboard.json"))
                                File.Delete("leaderboard.json");
                        }
                        catch
                        {
                        }
                        Console.Clear();
                        Console.WriteLine("Leaderboard has been reset.");
                    }
                    else
                    {
                        Console.WriteLine("Reset canceled.");
                    }
                    continue;
                }

                if (choice == "P")
                {
                    PlayRound();
                    continue;
                }

                Console.Clear();
                Console.WriteLine("Invalid selection.");
            }

            Console.Clear();
            Console.WriteLine($"Thanks for playing. Final balance: ${PocketMoney:F2}");
            return PocketMoney;
        }

        private void ShowUpgradesMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\nUpgrades - Balance: ${PocketMoney:F2}");
                Console.WriteLine("1) Buy +1 Luck (cost: $50)");
                Console.WriteLine("2) Increase win payout by $10 (cost: $200)");
                Console.WriteLine("3) Reduce play cost by $0.05 (cost: $100)");
                Console.WriteLine("4) Exit upgrades");
                Console.Write("Choose upgrade (1-4): ");
                string? upChoice = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(upChoice))
                    continue;
                upChoice = upChoice.Trim();
                if (upChoice == "4")
                    break;

                if (upChoice == "1")
                {
                    decimal cost = 50m;
                    if (PocketMoney < cost)
                        Console.WriteLine("Not enough money for that upgrade.");
                    else
                    {
                        PocketMoney -= cost;
                        luck += 1;
                        Console.WriteLine($"\nPurchased +1 Luck. Current luck: {luck}");
                        KickIfBroke();
                    }
                    continue;
                }

                if (upChoice == "2")
                {
                    decimal cost = 200m;
                    if (PocketMoney < cost)
                        Console.WriteLine("Not enough money for that upgrade.");
                    else
                    {
                        PocketMoney -= cost;
                        winAmount += 10m;
                        Console.WriteLine($"\nIncreased win payout by $10. Current win: ${winAmount:F2}");
                        KickIfBroke();
                    }
                    continue;
                }

                if (upChoice == "3")
                {
                    decimal cost = 100m;
                    if (PocketMoney < cost)
                        Console.WriteLine("Not enough money for that upgrade.");
                    else
                    {
                        PocketMoney -= cost;
                        rerollCost = Math.Max(0.05m, rerollCost - 0.05m);
                        playCost = Math.Max(0.05m, playCost - 0.05m);
                        Console.WriteLine($"Reduced reroll cost by $0.05. Current reroll cost: ${rerollCost:F2}" + $" | Play cost: ${playCost:F2}");
                        KickIfBroke();
                    }
                    continue;
                }

                Console.WriteLine("Invalid selection.");
            }
        }

        private void PlayRound()
        {
            if (PocketMoney < playCost)
            {
                Console.WriteLine("Not enough money to play.");
                return;
            }

            PocketMoney -= playCost;
            int randomNumber = RandomProvider.GetInt(1, maxRoll + 1);
            randomNumber = Math.Min(randomNumber + luck, maxRoll);

            bool isWin = randomNumber == maxRoll || (randomNumber >= RangeWinMin && randomNumber <= RangeWinMax) || (randomNumber >= RangeWin2Min && randomNumber <= RangeWin2Max);
            _leaderboard.RecordRoll(_playerName, isWin);

            Console.Clear();
            if (randomNumber == maxRoll)
            {
                PocketMoney += winAmount;;
                Console.WriteLine($"{_playerName} rolled {randomNumber} - YOU WIN +${winAmount:F2}!");
                return;
            }
            Console.WriteLine($"{_playerName} rolled {randomNumber} - YOU LOST -${playCost:F2}.");

            // Range special: lose $5 if roll is between 20 and 30
            if (randomNumber >= RangeLoseMin && randomNumber <= RangeLoseMax)
            {
                PocketMoney -= RangeLoseAmount;
                Console.WriteLine($"{_playerName}, {RangeLoseMessage} -${RangeLoseAmount:F2} (new balance: ${PocketMoney:F2})");
            }

            // Range special: lose $10 if roll is between 75 and 85
            if (randomNumber >= RangeLose2Min && randomNumber <= RangeLose2Max)
            {
                decimal actualLoss = Math.Min(RangeLose2Amount, PocketMoney);
                PocketMoney -= actualLoss;
                Console.WriteLine($"{_playerName}, {RangeLose2Message} -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
            }

            // Special 50: lose half
            if (randomNumber == SpecialNumber)
            {
                decimal half = Math.Floor(PocketMoney / 2m * 100m) / 100m;
                PocketMoney -= half;
                Console.WriteLine($"{SpecialMessage} {SpecialLoseHalfMessage} -${half:F2} (new balance: ${PocketMoney:F2})");
            }

            // Offer rerolls
            while (true)
            {
                Console.WriteLine($"Current balance: ${PocketMoney:F2}. Press Enter to reroll for ${rerollCost:F2} (or type N to stop)");
                string? rerollAns = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(rerollAns))
                {
                    rerollAns = rerollAns.Trim().ToUpperInvariant();
                    if (rerollAns == "N")
                        break;
                    if (rerollAns != "Y")
                        break; // treat anything else as stop
                }

                if (PocketMoney < rerollCost)
                {
                    Console.WriteLine("Not enough money to reroll.");
                    break;
                }

                PocketMoney -= rerollCost;
                randomNumber = RandomProvider.GetInt(1, maxRoll + 1);
                randomNumber = Math.Min(randomNumber + luck, maxRoll);

                bool rerollWin = randomNumber == maxRoll || (randomNumber >= RangeWinMin && randomNumber <= RangeWinMax) || (randomNumber >= RangeWin2Min && randomNumber <= RangeWin2Max);
                _leaderboard.RecordRoll(_playerName, rerollWin);

                if (randomNumber == maxRoll)
                {
                    PocketMoney += winAmount;
                    Console.WriteLine($"{_playerName} rerolled {randomNumber} - YOU FOUND $100 In YOUR MOMS PURSE +${winAmount:F2}!");
                    break;
                }

                if (randomNumber >= RangeWinMin && randomNumber <= RangeWinMax)
                {
                    PocketMoney += RangeWinAmount;
                    Console.WriteLine($"{_playerName} rerolled {randomNumber} - YOUR MOM GAVE YOU +${RangeWinAmount:F2}!");
                    continue;
                }

                if (randomNumber >= RangeWin2Min && randomNumber <= RangeWin2Max)
                {
                    PocketMoney += RangeWin2Amount;
                    Console.WriteLine($"{_playerName} rerolled {randomNumber} - You got your paycheck of +${RangeWin2Amount:F2}!");
                    continue;
                }

                Console.WriteLine($"{_playerName} rerolled {randomNumber} - YOU LOST -${rerollCost:F2}.");

                if (randomNumber >= RangeLoseMin && randomNumber <= RangeLoseMax)
                {
                    PocketMoney -= RangeLoseAmount;
                    Console.WriteLine($"{_playerName}, {RangeLoseMessage} -${RangeLoseAmount:F2} (new balance: ${PocketMoney:F2})");
                }

                if (randomNumber >= RangeLose2Min && randomNumber <= RangeLose2Max)
                {
                    decimal actualLoss = Math.Min(RangeLose2Amount, PocketMoney);
                    PocketMoney -= actualLoss;
                    Console.WriteLine($"{_playerName}, {RangeLose2Message} -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
                }

                if (randomNumber == SpecialNumber)
                {
                    decimal half = Math.Floor(PocketMoney / 2m * 100m) / 100m;
                    PocketMoney -= half;
                    Console.WriteLine($"{SpecialMessage} {SpecialLoseHalfMessage} -${half:F2} (new balance: ${PocketMoney:F2})");
                }
            }
        }
    }
}
