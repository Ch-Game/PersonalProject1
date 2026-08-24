using System.Security.Cryptography;

decimal PocketMoney = 1000m;
const decimal PlayCost = 0.10m;
const decimal RerollCost = 0.10m;
const decimal WinAmount = 100m;
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
const string RangeWinMessage = "You rolled between 51 and 55 — You won $15.00!";


Console.WriteLine("Welcome to the RNG game.");
while (true)
{
    Console.WriteLine($"\nYou have ${PocketMoney} in your pocket.");
    Console.WriteLine("Choose: (P)lay, (Q)uit");
    string? choice = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(choice))
        continue;

    choice = choice.Trim().ToUpperInvariant();
    if (choice == "Q")
        break;

    if (choice != "P")
        continue;

    // Charge for the play
    if (PocketMoney < PlayCost)
    {
        Console.WriteLine("Not enough money to play.");
        continue;
    }

    PocketMoney -= PlayCost;
    int randomNumber = RandomNumberGenerator.GetInt32(1, 101); // 1..100 inclusive
    if (randomNumber == 1)
    {
        PocketMoney += WinAmount;
        Console.WriteLine($"You rolled {randomNumber} - YOU WIN +${WinAmount:F2}!");
    }
    else
    {
        Console.WriteLine($"You rolled {randomNumber} - YOU SUCK -${PlayCost:F2}.");
    }

    if (randomNumber == SpecialNumber)
    {
        Console.WriteLine(SpecialMessage);
        // Special effect: lose half your money
        decimal lost = Math.Floor(PocketMoney / 2m * 100m) / 100m; // round down to cents
        PocketMoney -= lost;
        Console.WriteLine(SpecialLoseHalfMessage + $" -${lost:F2} (new balance: ${PocketMoney:F2})");
    }

    // Range special: lose $5 if roll is between 20 and 30
    if (randomNumber >= RangeLoseMin && randomNumber <= RangeLoseMax)
    {
        decimal actualLoss = Math.Min(RangeLoseAmount, PocketMoney);
        PocketMoney -= actualLoss;
        Console.WriteLine(RangeLoseMessage + $" -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
    }

    // Range special: lose $10 if roll is between 75 and 85
    if (randomNumber >= RangeLose2Min && randomNumber <= RangeLose2Max)
    {
        decimal actualLoss = Math.Min(RangeLose2Amount, PocketMoney);
        PocketMoney -= actualLoss;
        Console.WriteLine(RangeLose2Message + $" -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
    }

    // Offer rerolls until user says no or money runs out
    while (true)
    {
        Console.WriteLine($"Current balance: ${PocketMoney:F2}. Reroll for ${RerollCost:F2}? (Y/N)");
        string? rerollAns = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(rerollAns))
            break;
        rerollAns = rerollAns.Trim().ToUpperInvariant();
        if (rerollAns != "Y")
            break;

        if (PocketMoney < RerollCost)
        {
            Console.WriteLine("Not enough money to reroll.");
            break;
        }

        PocketMoney -= RerollCost;
        randomNumber = RandomNumberGenerator.GetInt32(1, 101);
        if (randomNumber == 100)
        {
            PocketMoney += WinAmount;
            Console.WriteLine($"Rerolled {randomNumber} - YOU WIN +${WinAmount:F2}!");
            break; // stop offering rerolls after a win
        }
        else
        {
            Console.WriteLine($"Rerolled {randomNumber} - YOU SUCK -${RerollCost:F2}.");
        }

        if (randomNumber == SpecialNumber)
        {
            Console.WriteLine(SpecialMessage);
            // Special effect: lose half your money
            decimal lost = Math.Floor(PocketMoney / 2m * 100m) / 100m; // round down to cents
            PocketMoney -= lost;
            Console.WriteLine(SpecialLoseHalfMessage + $" -${lost:F2} (new balance: ${PocketMoney:F2})");
        }
        // Range special: lose $5 if roll is between 20 and 30
        if (randomNumber >= RangeLoseMin && randomNumber <= RangeLoseMax)
        {
            decimal actualLoss = Math.Min(RangeLoseAmount, PocketMoney);
            PocketMoney -= actualLoss;
            Console.WriteLine(RangeLoseMessage + $" -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
        }
        // Range special: lose $10 if roll is between 75 and 85
        if (randomNumber >= RangeLose2Min && randomNumber <= RangeLose2Max)
        {
            decimal actualLoss = Math.Min(RangeLose2Amount, PocketMoney);
            PocketMoney -= actualLoss;
            Console.WriteLine(RangeLose2Message + $" -${actualLoss:F2} (new balance: ${PocketMoney:F2})");
        }
        // Range special: win $15 if roll is between 51 and 55
        if (randomNumber >= RangeWinMin && randomNumber <= RangeWinMax)
        {
            PocketMoney += RangeWinAmount;
            Console.WriteLine(RangeWinMessage + $" +${RangeWinAmount:F2} (new balance: ${PocketMoney:F2})");
        }

    }
}

Console.WriteLine($"Thanks for playing. Final balance: ${PocketMoney:F2}");
