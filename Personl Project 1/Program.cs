using System.Security.Cryptography;

decimal PocketMoney = 1000m;
// Mutable game configuration (can be changed by upgrades)
decimal playCost = 1m;
decimal rerollCost = 0.10m;
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

Console.WriteLine("Welcome to the RNG game.");
Console.Write("Enter your name: ");
string? playerName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(playerName))
    playerName = "Player";
playerName = playerName.Trim();
while (true)
{
    Console.WriteLine($"\nYou have ${PocketMoney:F2} in your pocket.");
    Console.WriteLine("Choose: (P)lay, (U)pgrades, (Q)uit");
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
            Console.WriteLine("3) Reduce play cost by $0.10 (cost: $100)");
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
                    playCost = Math.Max(0.01m, playCost - 0.01m);
                    Console.WriteLine($"Reduced play cost by $0.01. Current play cost: ${playCost:F2}");
                }
                continue;
            }

            Console.WriteLine("Invalid selection.");
        }

        continue; // return to main menu
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
        randomNumber = RandomNumberGenerator.GetInt32(1, maxRoll + 1);
        randomNumber = Math.Min(randomNumber + luck, maxRoll);

        // Handle absolute win (max roll) first and stop rerolls
        if (randomNumber == maxRoll)
        {
            PocketMoney += winAmount;
            Console.WriteLine($"{playerName} rerolled {randomNumber} - YOU WIN +${winAmount:F2}!");
            break; // stop offering rerolls after a win
        }

        // Handle the small-range win (51-55). Print only the "YOU WIN" line (no extra message)
        if (randomNumber >= RangeWinMin && randomNumber <= RangeWinMax)
        {
            PocketMoney += RangeWinAmount;
            Console.WriteLine($"{playerName} rerolled {randomNumber} - YOU WIN +${RangeWinAmount:F2}!");
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
