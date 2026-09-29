using PersonalProject1.Models;
using PersonalProject1.Services;

Console.WriteLine("Welcome to the RNG game.");
string? playerName;
do
{
    Console.Write("Enter your name: ");
    playerName = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(playerName))
    {
        Console.Clear();
        Console.WriteLine("Name cannot be empty. Please enter a name.");
    }
} while (string.IsNullOrWhiteSpace(playerName));
playerName = playerName!.Trim();
if (string.Equals(playerName, "you", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("You have been kicked out of the game called LIFE.");
    return;
}

var leaderboard = Leaderboard.Load("leaderboard.json");
var game = new GameService(playerName, leaderboard);
game.Run();
