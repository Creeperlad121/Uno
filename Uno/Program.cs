using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno
{
    internal class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("How many Players?");
                string input = Console.ReadLine();
                Console.Clear();
                if (int.TryParse(input, out Data.numberofplayers))
                {
                    for (int j = 1; j <= Data.numberofplayers; j++)
                    {
                        Data.playerhand.Add(new List<string>());
                        for (int i = 0; i < 7; i++)
                        {
                            Data.DrawCard(true, j);
                        }
                    }
                    int player = 0;
                    while (true)
                    {
                        int direction = Data.reverse ? -1 : 1;
                        player += direction;
                        if (player > Data.numberofplayers) { player = 1; }
                        if (player < 1) { player = Data.numberofplayers; }
                        Console.WriteLine($"Player {player}\n\nPlayer Hands");
                        Data.Score();
                        Console.WriteLine("\nCard in Play");
                        Data.DisplayCard(Data.currentcard);
                        Console.WriteLine("\n");
                        if (Data.playerhand.Count != 0)
                        {
                            Console.WriteLine("Press any key to play your turn");
                            Console.ReadKey();
                            Data.DisplayHand(player);
                            Data.PlayTurn(player);
                            Data.EndGame(player);
                        }
                        Console.Clear();
                    }
                }
                else
                { Console.WriteLine("Error, Enter a number"); }
            }
        }
    }
}