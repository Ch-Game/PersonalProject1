using System;
using PersonalProject1.Utilities;
using System.Diagnostics;
using System.Threading;

namespace PersonalProject1.Services
{
    public class SecretQuest
    {
        // Accepts the GameService instance as used in GameService.cs
        public void TryStart(GameService game)
        {
            Console.Clear();
            Console.WriteLine("--- SECRET QUEST: THE FORSAKEN ARENA ---\n");

            int playerHp = 100;
            int enemyHp = 500;
            int turn = 1;

            Console.WriteLine("You have been pulled into the Forsaken Arena. Defeat the enemy by rolling numbers 1-10. Each number has a different effect.");
            Console.WriteLine("Press Enter to begin...");
            Console.ReadLine();

            while (playerHp > 0 && enemyHp > 0)
            {
                Console.Clear();
                Console.WriteLine($"Turn {turn}");
                Console.WriteLine($"Player HP: {playerHp}    Enemy HP: {enemyHp}\n");
                Console.WriteLine("Press Enter to roll (1-10)...");
                Console.ReadLine();

                int roll = RandomProvider.GetInt(1, 11);
                Console.WriteLine($"You rolled: {roll}");

                switch (roll)
                {
                    case 1:
                        Console.WriteLine("Critical Strike! Enemy takes 8 damage.");
                        enemyHp -= 8;
                        break;
                    case 2:
                        Console.WriteLine("Heavy Blow! Enemy takes 6 damage.");
                        enemyHp -= 6;
                        break;
                    case 3:
                        Console.WriteLine("Solid Hit. Enemy takes 4 damage.");
                        enemyHp -= 4;
                        break;
                    case 4:
                        Console.WriteLine("Glancing blow. Enemy takes 2 damage.");
                        enemyHp -= 2;
                        break;
                    case 5:
                        Console.WriteLine("You missed.");
                        break;
                    case 6:
                        Console.WriteLine("Warmth of the Ancients. You heal 5 HP.");
                        playerHp += 5;
                        break;
                    case 7:
                        Console.WriteLine("The enemy steadies itself and regains 3 HP.");
                        enemyHp += 3;
                        break;
                    case 8:
                        Console.WriteLine("Enemy counterattack! You take 3 damage.");
                        playerHp -= 3;
                        break;
                    case 9:
                        // powerful attack with backfire chance
                        bool backfire = (RandomProvider.GetInt(0, 100) < 30); // 30% backfire
                        if (backfire)
                        {
                            Console.WriteLine("Power Strike backfired! You take 5 damage.");
                            playerHp -= 5;
                        }
                        else
                        {
                            Console.WriteLine("Power Strike! Enemy takes 10 damage.");
                            enemyHp -= 10;
                        }
                        break;
                    case 10:
                        if (enemyHp <= 10)
                        {
                            Console.WriteLine("Finishing Blow! Instant kill succeeded.");
                            enemyHp = 0;
                        }
                        else
                        {
                            Console.WriteLine("Finishing Blow failed. Enemy takes 3 damage.");
                            enemyHp -= 3;
                        }
                        break;
                }

                // Clamp HP
                if (playerHp > 100) playerHp = 100;
                if (enemyHp > 500) enemyHp = 500;

                if (enemyHp <= 0) break;

                // Enemy turn: directional attack roll (1-5)
                int attackRoll = RandomProvider.GetInt(1, 6); // 1-5
                // Map 1-4 to directions U/D/L/R. 5 is a heavy unblockable attack.
                string expectedKey = attackRoll switch
                {
                    1 => "U",
                    2 => "D",
                    3 => "L",
                    4 => "R",
                    _ => ""
                };

                int enemyDamage = (attackRoll == 5) ? 5 : 2;

                if (attackRoll == 5)
                {
                    Console.WriteLine("\nEnemy uses a heavy attack! This one cannot be defended.");
                    Console.WriteLine($"It deals {enemyDamage} damage to you.");
                    playerHp -= enemyDamage;
                }
                else
                {
                    Console.WriteLine($"\nEnemy attacks from {(expectedKey == "U" ? "above" : expectedKey == "D" ? "below" : expectedKey == "L" ? "the left" : "the right")}!");
                    int timeoutMs = 3000; // time allowed to defend
                    Console.WriteLine($"Quick! Press U (up), D (down), L (left) or R (right) to defend. You have {timeoutMs / 1000} seconds...");

                    var sw = Stopwatch.StartNew();
                    ConsoleKeyInfo keyInfo = default;
                    bool gotInput = false;

                    while (sw.ElapsedMilliseconds < timeoutMs)
                    {
                        if (Console.KeyAvailable)
                        {
                            keyInfo = Console.ReadKey(true);
                            gotInput = true;
                            break;
                        }
                        Thread.Sleep(50);
                    }

                    if (!gotInput)
                    {
                        Console.WriteLine("\nTime ran out!");
                        Console.WriteLine($"Defense failed. You take {enemyDamage} damage.");
                        playerHp -= enemyDamage;
                    }
                    else
                    {
                        char pressed = char.ToUpperInvariant(keyInfo.KeyChar);
                        Console.WriteLine($"You pressed: {pressed}");

                        if (pressed.ToString() == expectedKey)
                        {
                            Console.WriteLine("You successfully defended the attack and took no damage!");
                        }
                        else
                        {
                            Console.WriteLine($"Defense failed. You take {enemyDamage} damage.");
                            playerHp -= enemyDamage;
                        }
                    }
                }

                if (playerHp <= 0) break;

                Console.WriteLine("\nPress Enter to continue to next turn...");
                Console.ReadLine();
                turn++;
            }

            Console.WriteLine();
            if (playerHp > 0 && enemyHp <= 0)
            {
                Console.WriteLine("You have defeated the enemy! The secret quest is complete.");
            }
            else
            {
                Console.WriteLine("You have been defeated in the Forsaken Arena. Game over.");
            }

            Console.WriteLine("Press Enter to exit the game.");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
