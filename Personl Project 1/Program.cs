using System.Security.Cryptography;

decimal PocketMoney = 1000m;
// Mutable game configuration (can be changed by upgrades)
decimal playCost = 0.50m;
decimal rerollCost = 0.50m;

decimal winAmount = 100m;

int maxRoll = 100; // highest possible roll

int luck = 0;      // added to each roll (capped at maxRoll)

const int SpecialNumber = 50;
const string SpecialMessage = "You rolled 50 — A Indian scammer stole your money!";
const string SpecialLoseHalfMessage = "Special effect: you lose half your money";

const int RangeLoseMin = 20;
const int RangeLoseMax = 30;
const decimal RangeLoseAmount = 5m;
const string RangeLoseMessage = "You rolled between 20 and 30 — Your mom stole $5.00 from you.";

const int RangeLose2Min = 75;
const int RangeLose2Max = 85;
const decimal RangeLose2Amount = 10m;
const string RangeLose2Message = "You rolled between 75 and 85 — Mr. Preston asked for your Tutition Money You lose $10.00.";

const int RangeWinMin = 51;
const int RangeWinMax = 55;
const decimal RangeWinAmount = 15m;

const int RangeWin2Min = 93;
const int RangeWin2Max = 90;
const decimal RangeWin2Amount = 25m;



Console.WriteLine("Welcome to the RNG game.");
Console.Write("Enter your name: ");
string? playerName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(playerName))
    playerName = "Player";
playerName = playerName.Trim();
// Auto-kick rule: immediately exit the game if the player identifies as "you"
if (string.Equals(playerName, "you", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("You have been kicked out of the game called LIFE.");
    return; // end the program
}
// Load leaderboard (saves to leaderboard.json in working directory)
var leaderboard = Leaderboard.Load("leaderboard.json");
// Helper: exit the program if the player has no money left
void KickIfBroke()
{
    if (PocketMoney <= 0m)
    {
        Console.WriteLine("You have been kicked out of the game called LIFE.");
        Environment.Exit(0);
    }
}
while (true)
{
    Console.WriteLine($"\nYou have ${PocketMoney:F2} in your pocket.");
    KickIfBroke();
    Console.WriteLine("Choose: (P)lay, (U)pgrades, (L)eaderboard, (R)eset leaderboard, (Q)uit");
    string? choice = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(choice))
        continue;

    choice = choice.Trim().ToUpperInvariant();
    if (choice == "Q")
        break;

    if (choice == "U")
    {
        // Upgrades menu
        while (true)
        {
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
                    Console.WriteLine($"Purchased +1 Luck. Current luck: {luck}");
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
                    Console.WriteLine($"Increased win payout by $10. Current win: ${winAmount:F2}");
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

        continue; // return to main menu
    }

    if (choice == "L")
    {
        Console.WriteLine("\nLeaderboard (top players by wins):");
        foreach (var e in leaderboard.Top(20))
        {
            Console.WriteLine($"{e.Name} - Rolls: {e.Rolls}, Wins: {e.Wins}, Losses: {e.Losses}");
        }
        continue;
    }

    if (choice == "R")
    {
        Console.Write("Are you sure you want to reset the leaderboard? This cannot be undone. (Y/N): ");
        string? confirm = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(confirm) && confirm.Trim().ToUpperInvariant() == "Y")
        {
            // reset in-memory and on-disk
            try
            {
                // clear in-memory entries via reflection-like access using save of empty list
                // create a new empty file or delete existing file
                if (File.Exists("leaderboard.json"))
                    File.Delete("leaderboard.json");
            }
            catch
            {
                // ignore errors
            }
            Console.WriteLine("Leaderboard has been reset.");
        }
        else
        {
            Console.WriteLine("Reset canceled.");
        }
        // reload leaderboard object to ensure in-memory state matches disk
        leaderboard = Leaderboard.Load("leaderboard.json");
        continue;
    }

    if (choice != "P")
        continue;

    // Charge for the play
    if (PocketMoney < playCost)
    {
        Console.WriteLine("Not enough money to play.");
        continue;
    }

    PocketMoney -= playCost;
    int randomNumber = RandomNumberGenerator.GetInt32(1, maxRoll + 1); // 1..maxRoll inclusive
    // apply luck as a bonus toward higher numbers
    randomNumber = Math.Min(randomNumber + luck, maxRoll);
    // determine whether this roll counts as a win for leaderboard purposes
    bool isWin = randomNumber == maxRoll || (randomNumber >= RangeWinMin && randomNumber <= RangeWinMax) || (randomNumber >= RangeWin2Min && randomNumber <= RangeWin2Max);
    leaderboard.RecordRoll(playerName, isWin);
    if (randomNumber == maxRoll)
    {
        PocketMoney += winAmount;
        Console.WriteLine($"{playerName} rolled {randomNumber} - YOU WIN +${winAmount:F2}!");
    }
    else
    {
        Console.WriteLine($"{playerName} rolled {randomNumber} - YOU LOST -${playCost:F2}.");
    }

    if (randomNumber == SpecialNumber)
    {
        Console.WriteLine($"{playerName}, {SpecialMessage}");
        // Special effect: lose half your money
        decimal lost = Math.Floor(PocketMoney / 2m * 100m) / 100m; // round down to cents
        PocketMoney -= lost;
        Console.WriteLine($"{playerName}, {SpecialLoseHalfMessage} -${lost:F2} (new balance: ${PocketMoney:F2})");
        KickIfBroke();
    }

    // Range special: lose $5 if roll is between 20 and 30
    if (randomNumber >= RangeLoseMin && randomNumber <= RangeLoseMax)
    {
        decimal actualLoss = Math.Min(RangeLoseAmount, PocketMoney);
        PocketMoney -= actualLoss;
        Console.WriteLine($"{playerName}, {RangeLoseMessage} -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
        KickIfBroke();
    }

    // Range special: lose $10 if roll is between 75 and 85
    if (randomNumber >= RangeLose2Min && randomNumber <= RangeLose2Max)
    {
        decimal actualLoss = Math.Min(RangeLose2Amount, PocketMoney);
        PocketMoney -= actualLoss;
        Console.WriteLine($"{playerName}, {RangeLose2Message} -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
        KickIfBroke();
    }

    // Offer rerolls until user says no or money runs out
    while (true)
    {
        Console.WriteLine($"Current balance: ${PocketMoney:F2}. Press Enter to reroll for ${rerollCost:F2} (or type N to stop)");
        string? rerollAns = Console.ReadLine();
        // Treat empty input (Enter) as confirmation to reroll. Accept 'Y' as well. 'N' stops.
        if (!string.IsNullOrWhiteSpace(rerollAns))
        {
            rerollAns = rerollAns.Trim().ToUpperInvariant();
            if (rerollAns == "N")
                break;
            if (rerollAns != "Y")
                break;
        }

        if (PocketMoney < rerollCost)
        {
            Console.WriteLine("Not enough money to reroll.");
            break;
        }

        PocketMoney -= rerollCost;
        KickIfBroke();
        randomNumber = RandomNumberGenerator.GetInt32(1, maxRoll + 1);
        randomNumber = Math.Min(randomNumber + luck, maxRoll);
        // record this reroll
        bool rerollWin = randomNumber == maxRoll || (randomNumber >= RangeWinMin && randomNumber <= RangeWinMax) || (randomNumber >= RangeWin2Min && randomNumber <= RangeWin2Max);
        leaderboard.RecordRoll(playerName, rerollWin);

        // Handle absolute win (max roll) first and stop rerolls
        if (randomNumber == maxRoll)
        {
            PocketMoney += winAmount;
            Console.WriteLine($"{playerName} rerolled {randomNumber} - YOU FOUND $100 In YOUR MOMS PURSE +${winAmount:F2}!");
            break; // stop offering rerolls after a win
        }

        // Handle the small-range win (51-55).
        if (randomNumber >= RangeWinMin && randomNumber <= RangeWinMax)
        {
            PocketMoney += RangeWinAmount;
            Console.WriteLine($"{playerName} rerolled {randomNumber} - YOUR MOM GAVE YOU +${RangeWinAmount:F2}!");
            // continue offering rerolls (do not print the earlier "YOU LOST" line)
            continue;
        }

        // Handle the small-range win (90 - 93).
        if (randomNumber >= RangeWin2Min && randomNumber <= RangeWin2Max)
        {
            PocketMoney += RangeWin2Amount;
            Console.WriteLine($"{playerName} rerolled {randomNumber} - You got your paycheck of +${RangeWin2Amount:F2}!");
            // continue offering rerolls (do not print the earlier "YOU LOST" line)
            continue;
        }

        // Otherwise it's a loss for the reroll; show lost message then apply losing specials
        Console.WriteLine($"{playerName} rerolled {randomNumber} - YOU LOST -${rerollCost:F2}.");

        if (randomNumber == SpecialNumber)
        {
            Console.WriteLine($"{playerName}, {SpecialMessage}");
            // Special effect: lose half your money
            decimal lost = Math.Floor(PocketMoney / 2m * 100m) / 100m; // round down to cents
            PocketMoney -= lost;
            Console.WriteLine($"{playerName}, {SpecialLoseHalfMessage} -${lost:F2} (new balance: ${PocketMoney:F2})");
        }
        // Range special: lose $5 if roll is between 20 and 30
        if (randomNumber >= RangeLoseMin && randomNumber <= RangeLoseMax)
        {
            decimal actualLoss = Math.Min(RangeLoseAmount, PocketMoney);
            PocketMoney -= actualLoss;
            Console.WriteLine($"{playerName}, {RangeLoseMessage} -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
        }
        // Range special: lose $10 if roll is between 75 and 85
        if (randomNumber >= RangeLose2Min && randomNumber <= RangeLose2Max)
        {
            decimal actualLoss = Math.Min(RangeLose2Amount, PocketMoney);
            PocketMoney -= actualLoss;
            Console.WriteLine($"{playerName}, {RangeLose2Message} -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
        }

    }
}

Console.WriteLine($"Thanks for playing. Final balance: ${PocketMoney:F2}");

// Leaderboard types and persistence (declared after top-level statements)
public class LeaderboardEntry
{
    public string Name { get; set; } = string.Empty;
    public int Rolls { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
}

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
