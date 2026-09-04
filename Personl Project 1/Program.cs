using PersonalProject1.Models;
using PersonalProject1.Services;

Console.WriteLine("Welcome to the RNG game.");
Console.Write("Enter your name: ");
string? playerName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(playerName))
    playerName = "Player";
playerName = playerName.Trim();
if (string.Equals(playerName, "you", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("You have been kicked out of the game called LIFE.");
    return;
}

var leaderboard = Leaderboard.Load("leaderboard.json");
var game = new GameService(playerName, leaderboard);
game.Run();
