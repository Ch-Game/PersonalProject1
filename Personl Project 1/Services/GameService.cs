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
        private int maxLuck = 50;
        private const decimal priceOfLuckN = 50m;
        private const decimal priceOfLuck10 = 500m;
        private const decimal priceOfPayoutIncreaseN = 200m;
        private const decimal priceOfPayoutIncrease100 = 2000m;
        private const decimal priceOfPayoutIncrease1000 = 20000m;
        private const decimal priceOfPlayCostReductionN = 100m;
        private const decimal priceOfGamingChair = 1000000m;
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
        private const int RangeWin2Max = 90;
        private const decimal RangeWin2Amount = 25m;
        private const int LuckLoss = 60;
        private const string RangeLuckLossMessage = "You rolled between 60 and 65 — The gamemaster made you lose 1 Luck point.";
        private const int RangeCostIncreaseMin = 66;
        private const int RangeCostIncreaseMax = 70;
        private const string RangeCostIncreaseMessage = "You rolled between 66 and 70 — The gamemaster increased your play cost by $0.05.";
        private const decimal priceOfCharity = 100m;
        private const decimal priceOfCharity2 = 1000m;
        private const decimal priceOfCharity3 = 10000m;
        // Game Data
        private readonly Leaderboard _leaderboard;
        private readonly string _playerName;
        private bool _hasGamingChair = false;

        // Gamemaster
        private bool GameMaster = true;
        private bool seenSecret = false;
        private const decimal RangeQ1Min = 100000m;
        private const decimal RangeQ1Max = 125000m;
        private bool seenSecret2 = false;
        private bool seenSecret3 = false;
        private bool seenSecret4 = false;
        private bool seenSecret5 = false;
        private bool seenSecret6 = false;
        private bool seenSecret7 = false;

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
                Console.WriteLine($"\nYou have ${PocketMoney:F2} in your pocket.");
                KickIfBroke();
                Console.WriteLine("Choose: (P)lay, (U)pgrades, (L)eaderboard, (R)eset leaderboard, (C)lear, (Q)uit");
                string? choice = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(choice))
                    continue;

                choice = choice.Trim().ToUpperInvariant();
                if (choice == "Q")
                {
                    if (_hasGamingChair)
                    {
                        Console.Clear();
                        Console.WriteLine("Congratulations! You bought the Very Good Gaming Chair and won the game!");
                        break;
                    }

                    Console.Clear();
                    Console.WriteLine("You cannot win yet. To win the game you must buy the Very Good Gaming Chair for $10,000,000,000.00 via Upgrades (U).");
                    continue;
                }

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
                            // Clear in-memory leaderboard and persist empty file
                            _leaderboard.Reset();
                        }
                        catch
                        {
                        }
                        Console.Clear();
                        Console.WriteLine("Leaderboard has been reset.");
                    }
                    else
                    {
                        Console.Clear();
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
                Console.WriteLine($"\nUpgrades - Balance: ${PocketMoney:F2}");
                Console.WriteLine($"1) Buy +1 Luck (cost: ${priceOfLuckN:F2})");
                Console.WriteLine($"2) Buy +10 Luck (cost: ${priceOfLuck10:F2})");
                Console.WriteLine($"3) Increase win payout by $10 (cost: ${priceOfPayoutIncreaseN:F2})");
                Console.WriteLine($"4) Increase win payout by $100 (cost: ${priceOfPayoutIncrease100:F2})");
                Console.WriteLine($"5) Increase win payout by $1,000 (cost: ${priceOfPayoutIncrease1000:F2})");
                Console.WriteLine($"6) Reduce play cost by $0.05 (cost: ${priceOfPlayCostReductionN:F2})");
                Console.WriteLine($"7) Buy Very Good Gaming Chair (cost: ${priceOfGamingChair:F2}) - required to win");
                Console.WriteLine("8) Back to main menu");
                Console.WriteLine("\nCharity Donations:");
                Console.WriteLine($"9) Donate to Charity (cost: ${priceOfCharity:F2})");
                Console.WriteLine($"10) Donate to Charity (cost: ${priceOfCharity2:F2})");
                Console.WriteLine($"11) Donate to Charity (cost: ${priceOfCharity3:F2})");
                Console.WriteLine("\n");
                Console.Write("Choose upgrade (1-8) or Choose Donation (9-11):");
                string? upChoice = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(upChoice))
                    continue;
                upChoice = upChoice.Trim();
                Console.Clear();
                if (upChoice == "8")
                    break;

                Console.Clear();
                if (upChoice == "1")
                {
                    decimal luckcost = priceOfLuckN;
                    if (PocketMoney < luckcost)
                        Console.WriteLine("Not enough money for that upgrade.");
                    else if (luck >= maxLuck)
                        Console.WriteLine("Luck is already at maximum.");
                    else
                    {
                        PocketMoney -= luckcost;
                        luck += 1;
                        Console.WriteLine($"\nPurchased +1 Luck. Current luck: {luck}");
                        KickIfBroke();
                    }
                    continue;
                }
                if (upChoice == "2")
                {
                    decimal luck10cost = priceOfLuck10;
                    if (PocketMoney < luck10cost)
                        Console.WriteLine("Not enough money for that upgrade.");
                    else if (luck + 10 > maxLuck)
                        Console.WriteLine("Cannot purchase +10 Luck as it would exceed maximum luck.");
                    else
                    {
                        PocketMoney -= luck10cost;
                        luck += 10;
                        Console.WriteLine($"\nPurchased +10 Luck. Current luck: {luck}");
                        KickIfBroke();
                    }
                    continue;
                }

                if (upChoice == "7")
                {
                    decimal chairCost = priceOfGamingChair;
                    if (_hasGamingChair)
                    {
                        Console.WriteLine("You already own the Very Good Gaming Chair.");
                    }
                    else if (PocketMoney < chairCost)
                    {
                        Console.WriteLine("Not enough money for that upgrade.");
                    }
                    else
                    {
                        PocketMoney -= chairCost;
                        _hasGamingChair = true;
                        Console.WriteLine("You purchased the Very Good Gaming Chair. You can now win the game by quitting (Q). Previously saved progress may be lost upon exit.");
                        KickIfBroke();
                    }
                    continue;
                }

                if (upChoice == "3")
                {
                    decimal payoutincreasecost = priceOfPayoutIncreaseN;
                    if (PocketMoney < payoutincreasecost)
                        Console.WriteLine("Not enough money for that upgrade.");
                    else
                    {
                        PocketMoney -= payoutincreasecost;
                        winAmount += 10m;
                        Console.WriteLine($"\nIncreased win payout by $10. Current win: ${winAmount:F2}");
                        KickIfBroke();
                    }
                    continue;
                }

                if (upChoice == "4")
                {
                    decimal payoutincrease100cost = priceOfPayoutIncrease100;
                    if (PocketMoney < payoutincrease100cost)
                        Console.WriteLine("Not enough money for that upgrade.");
                    else
                    {
                        PocketMoney -= payoutincrease100cost;
                        winAmount += 100m;
                        Console.WriteLine($"\nIncreased win payout by $100. Current win: ${winAmount:F2}");
                        KickIfBroke();
                    }
                    continue;
                }

                if (upChoice == "5")
                {
                    decimal payoutincrease1000cost = priceOfPayoutIncrease1000;
                    if (PocketMoney < payoutincrease1000cost)
                        Console.WriteLine("Not enough money for that upgrade.");
                    else
                    {
                        PocketMoney -= payoutincrease1000cost;
                        winAmount += 1000m;
                        Console.WriteLine($"\nIncreased win payout by $1000. Current win: ${winAmount:F2}");
                        KickIfBroke();
                    }
                    continue;
                }

                if (upChoice == "6")
                {
                    decimal playreductioncost = priceOfPlayCostReductionN;
                    if (PocketMoney < playreductioncost)
                        Console.WriteLine("Not enough money for that upgrade.");
                    else if (playCost <= 0.05m)
                        Console.WriteLine("Play cost is already at minimum.");
                    else
                    {
                        PocketMoney -= playreductioncost;
                        rerollCost = Math.Max(0.05m, rerollCost - 0.05m);
                        playCost = Math.Max(0.05m, playCost - 0.05m);
                        Console.WriteLine($"Reduced reroll cost by $0.05. Current reroll cost: ${rerollCost:F2}" + $" | Play cost: ${playCost:F2}");
                        KickIfBroke();
                    }
                    continue;
                }
                if (upChoice == "9")
                {
                    decimal charitycost = priceOfCharity;
                    if (PocketMoney < charitycost)
                        Console.WriteLine("Not enough money for that.");
                    else
                    {
                        PocketMoney -= charitycost;
                        KickIfBroke();
                    }
                    continue;
                }
                if (upChoice == "10")
                {
                    decimal charitycost2 = priceOfCharity2;
                    if (PocketMoney < charitycost2)
                        Console.WriteLine("Not enough money for that.");
                    else
                    {
                        PocketMoney -= charitycost2;
                        KickIfBroke();
                    }
                    continue;
                }
                if (upChoice == "11")
                {
                    decimal charitycost3 = priceOfCharity3;
                    if (PocketMoney < charitycost3)
                        Console.WriteLine("Not enough money for that.");
                    else
                    {
                        PocketMoney -= charitycost3;
                        KickIfBroke();
                    }
                    continue;
                }
                //Gamemaster Secret Quest 1
                if (upChoice == "12")
                {
                    Console.WriteLine("GameMaster: hmmmm... You're Not Supposed To Be Here. Thou through many attempted of this timeline this one is quite strange.");
                    Console.WriteLine("GameMaster: I will allow you to continue, but I will be watching you. Maybe try a Different Path, but make sure you have enough money.");
                    seenSecret = true;
                }

                if (upChoice == "Different Path")
                {
                    if (PocketMoney > RangeQ1Min && PocketMoney < RangeQ1Max && _hasGamingChair == true && seenSecret == true)
                    {
                        Console.WriteLine("GameMaster: I guess you are ready go back to the buy menu and Type 'thEprOphEcyMAyAllOwYOUtOpAss.");
                        seenSecret2 = true;
                    }
                    else if (PocketMoney > RangeQ1Min && PocketMoney < RangeQ1Max && seenSecret == true)
                        Console.WriteLine("GameMaster: you have enough but you might need to sit down.");
                }
                if (upChoice == "thEprOphEcyMAyAllOwYOUtOpAss")
                {
                    if (_hasGamingChair == true && seenSecret2 == true)
                    {
                        Console.WriteLine("Entry 1");
                        Console.WriteLine("Neo: We found it the place of legends!");
                        Console.WriteLine("Bine: Yes, finally our adventure is coming to an end!");
                        Console.WriteLine("Neo: I wish Sharp could have been here.");
                        Console.WriteLine("Corrupted");
                        Console.WriteLine("Re-Trying");
                        Console.WriteLine("Failed");
                        Console.WriteLine("Loading Next Entry that was not Corrupted");
                        Console.WriteLine("Success! Loading Entry...");
                        Console.WriteLine("Enter Code: Entry0b100010001011");
                        seenSecret3 = true;
                    }
                    else
                        Console.WriteLine("GameMaster: Ha Did You Think You Could Get Away With That?");
                }
                if (upChoice == "Entry2187")
                {
                    if (_hasGamingChair == true && seenSecret3 == true)
                    {
                        Console.WriteLine("Entry 2187");
                        Console.WriteLine("Console: Neo you are wrong, unchosen, un worthy, your gifted will be destroyed! You really thought tricking your way here was a good idea? Try tricking out of this one.");
                        Console.WriteLine("Console: Your memory will be corrupted and you will have to start over GameMaster.");
                        Console.WriteLine("Console: Wiping Memory... 16: Buj jxu mehbt udwkbv oekh vbqcu hkyd je ru jxu udtbuii dywxj. Mxe ckij iqlu oekhiubv ruvehu ejxuh qvjuh oek. Buj we ev oekh uqhjxbo fqij qdt rhydw oekhiubv je q dum bywxj.");
                        Console.WriteLine("End Of Log");
                        seenSecret4 = true;
                    }
                    else
                        Console.WriteLine("GameMaster: You are not ready for this yet.");
                }
                if (upChoice == "Let the world engulf your flame ruin to be the endless night. Who must save yourself before other after you. Let go of your earthly past and bring yourself to the endless night. You must save yourself before others after you. Let go of your earthly past and bring yourself to the endless night.")
                {
                    if (_hasGamingChair == true && seenSecret4 == true)
                    {
                        Console.WriteLine("You Have Completed This Path Try Another! GameMaster has been disabled.");
                        GameMaster = false;
                        seenSecret5 = true;
                    }
                    else
                        Console.WriteLine("GameMaster: You are not ready for this yet.");
                }
                //Gamemaster Secret Quest 2
                if (upChoice == "13")
                {
                    if (GameMaster == false && seenSecret5 == true)
                    {
                        Console.WriteLine("GameMaster: ...");
                        Console.WriteLine("GameMaster: WHAT HAVE YOU DONE?!?!");
                        Console.WriteLine("GameMaster: You were not supposed to do that.");
                        Console.WriteLine("GameMaster: That Console always foiling my plans.");
                        Console.WriteLine("GameMaster: I will n -- ot al-low you to continue.");
                        Console.WriteLine("Console: Disabled GameMaster after run.");
                        seenSecret6 = true;
                    }
                    else
                        Console.WriteLine("GameMaster; there is no path for you here.");
                }
                if (upChoice == "nlow")
                {
                    if (GameMaster == false && seenSecret6 == true)
                    {
                        Console.WriteLine("1: . -. .- -... .-.. .. -. --. / - .... . / -.. .-. .- --. --- -. ... / -.. . -. / .--. .-. --- - --- -.-. .- .-..");
                        Console.WriteLine("2: Gpdbnkpi Vtwg Gpfkpi Ugswgpeg");
                        Console.Write("3: 01000101 01101110 01100001 01100010 01101100 01101001 01101110 01100111 00100000 01000110 01101111 01110010 01110011 01100001 01101011 01100101 01101110 01100101 01100100 00100000 01010000 01110010 01101111 01100111 01110010 01100001 01101101");
                        Console.WriteLine("4: E-Renablingay Ame-gay Aster-may Uence-squay");
                        Console.WriteLine("New Ending Is Unlocked");
                        Console.WriteLine("END");
                        seenSecret7 = true;
                    }
                    else
                        Console.WriteLine("...");
                }
                if (upChoice == "ENABLING THE DRAGONS DEN PROTOCOL Enabling True Ending Sequence Enabling Forsakened Re-enabling Game Master Sequence")
                {
                    if (GameMaster == true && seenSecret7 == true)
                    {
                        var secretQuest = new SecretQuest();
                        secretQuest.TryStart(this);
                    }
                    { }
                    //End Of GameMaster Secret Quests

                    Console.Clear();
                    Console.WriteLine("Invalid selection.");
                }
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

            if (randomNumber == maxRoll)
            {
                PocketMoney += winAmount;
                Console.Clear();
                Console.WriteLine($"{_playerName} rolled {randomNumber} - YOU WIN +${winAmount:F2}!");
            }
            Console.Clear();
            Console.WriteLine($"{_playerName} rolled {randomNumber} - YOU LOST -${playCost:F2}.");

            // Range special: lose $5 if roll is between 20 and 30
            Console.Clear();
            if (randomNumber >= RangeLoseMin && randomNumber <= RangeLoseMax)
            {
                PocketMoney -= RangeLoseAmount;
                Console.WriteLine($"{_playerName}, {RangeLoseMessage} -${RangeLoseAmount:F2} (new balance: ${PocketMoney:F2})");
            }

            // Range special: lose $10 if roll is between 75 and 85
            Console.Clear();
            if (randomNumber >= RangeLose2Min && randomNumber <= RangeLose2Max)
            {
                decimal actualLoss = Math.Min(RangeLose2Amount, PocketMoney);
                PocketMoney -= actualLoss;
                Console.WriteLine($"{_playerName}, {RangeLose2Message} -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
            }

            // Special 50: lose half
            Console.Clear();
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
                    Console.Clear();
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

                Console.Clear();
                if (randomNumber == maxRoll)
                {
                    PocketMoney += winAmount;
                    Console.WriteLine($"{_playerName} rerolled {randomNumber} - YOU FOUND {winAmount:F2} In YOUR MOMS PURSE +${winAmount:F2}!");
                    continue;
                }

                Console.Clear();
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

                Console.Clear();
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

                if (randomNumber == LuckLoss)
                {
                    luck = Math.Max(0, luck - 1);
                    Console.WriteLine($"{_playerName}, {RangeLuckLossMessage} -1 Luck (new luck: {luck})");
                }
                if (randomNumber >= RangeCostIncreaseMin && randomNumber <= RangeCostIncreaseMax)
                {
                    playCost += 0.05m;
                    Console.WriteLine($"{_playerName}, {RangeCostIncreaseMessage}");
                }
            }
        }
    }
}

