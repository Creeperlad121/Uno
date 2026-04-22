using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno
{
    internal static class Data
    {
        public static Random random = new();
        public static string[] cards = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Draw 2", "Reverse", "Skip", "Wild", "Wild +4"];
        public static string currentcard = random.Next(4) + "," + cards[random.Next(10)] + ",0";
        public static List<List<string>> playerhand = [[]];
        public static List<int> winner = [];
        public static int numberofplayers;
        public static bool aiplayer;
        public static bool reverse = false;
        public static bool skip = false;

        public static void DrawCard(bool righttohand, int player)
        {
            string draw = cards[random.Next(cards.Length)];
            switch (draw)
            {
                case "Draw 2":
                    draw = random.Next(4) + "," + draw + ",2";
                    break;
                case "Wild":
                    draw = "4," + draw + ",0";
                    break;
                case "Wild +4":
                    draw = "4," + draw + ",4";
                    break;
                default:
                    draw = random.Next(4) + "," + draw + ",0";
                    break;
            }
            if (!righttohand)
            {
                Console.Write("Drawn card is: ");
                DisplayCard(draw);
                string[] ccparts = currentcard.Split(',');
                string[] parts = draw.Split(',');
                if (parts[0] == ccparts[0] || parts[1] == "Wild" || parts[1] == ccparts[1] || parts[1] == "Wild +4")
                {
                    string input = "";
                    if (!aiplayer)
                    {
                        Console.WriteLine("\nDo you want to play it? Y or N");
                        input = Console.ReadLine().ToLower();
                    }
                    else
                    {
                        input = "y";
                    }
                    switch (input)
                    {
                        case "y":
                        case "yes":
                        case "1":
                            PlayCard(draw, 0, player);
                            return;
                    }
                }
            }
            playerhand[player].Add(draw);
            playerhand[player].Sort();
            if (righttohand) { PlayCard("4,None,0", 0, player); }
        }

        public static void DisplayCard(string card)
        {
            string[] parts = card.Split(',');
            switch (Convert.ToInt32(parts[0]))
            {
                case 0:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.Write(parts[1]);
            Console.ResetColor();
        }


        public static void DisplayHand(int player)
        {
            Console.Write("Your hand\n| ");
            foreach (var card in playerhand[player])
            {
                DisplayCard(card);
                Console.Write(" | ");
            }
            Console.WriteLine();
        }

        public static List<string> ActionOptions(int player)
        {
            List<string> useablecards = [];
            string[] ccparts = currentcard.Split(',');
            Console.WriteLine("\nPlayable actions are");
            int c = 0;
            Console.Write($"{c}: Draw a card");
            foreach (string card in playerhand[player])
            {
                string[] parts = card.Split(',');
                if (ccparts[2] == "0")
                {
                    if (parts[0] == ccparts[0] || parts[1] == "Wild" || parts[1] == ccparts[1] || parts[1] == "Wild +4")
                    {
                        c++;
                        Console.Write($"\n{c}: ");
                        DisplayCard(card);
                        useablecards.Add(card);
                    }
                }
                else
                {
                    if (parts[1] == ccparts[1] || parts[1] == "Wild +4")
                    {
                        c++;
                        Console.Write($"\n{c}: ");
                        DisplayCard(card);
                        useablecards.Add(card);
                    }
                }
            }
            return useablecards;
        }

        public static void PlayAction(List<string> useablecards, int drawnumber, int player)
        {
            bool accept = false;
            while (accept == false)
            {
                string input = "";
                if (aiplayer)
                {
                    if (useablecards.Count == 0) { input = "0"; }
                    else
                        input = random.Next(useablecards.Count - 1) + 1.ToString();
                }
                else
                {
                    Console.WriteLine("\n\nWhich Action you play?");
                    input = Console.ReadLine();
                }
                if (int.TryParse(input, out int whichcard))
                {
                    if (whichcard < 0 || whichcard > useablecards.Count)
                    {
                        Console.WriteLine("Invalid Number");
                    }
                    else if (whichcard is 0)
                    {
                        bool hand = false;
                        if (drawnumber > 0)
                        {
                            hand = true;
                            drawnumber--;
                        }
                        for (int i = 0; i <= drawnumber; i++)
                        {
                            DrawCard(hand, player);
                        }
                        accept = true;
                    }
                    else
                    {
                        whichcard--;
                        PlayCard(useablecards[whichcard], drawnumber, player);
                        useablecards.Clear();
                        accept = true;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Number, try again");
                }

            }
        }

        public static void PlayCard(string playcard, int drawnumber, int player)
        {
            foreach (string card in playerhand[player]) { if (card == playcard) { playerhand[player].Remove(card); break; } }
            string[] strings = playcard.Split(',');
            switch (strings[1])
            {
                case "Reverse":
                    reverse = !reverse;
                    if (numberofplayers == 2) skip = true;
                    break;
                case "Draw 2":
                    drawnumber += 2;
                    break;
                case "Wild":
                    strings[0] = PickColor();
                    break;
                case "Skip":
                    skip = true;
                    break;
                case "Wild +4":
                    strings[0] = PickColor();
                    drawnumber += 4;
                    break;
                case "None":
                    strings = currentcard.Split(',');
                    break;
            }
            currentcard = $"{strings[0]},{strings[1]},{drawnumber}";
        }

        public static void PlayTurn(int player)
        {
            string[] ccparts = currentcard.Split(',');
            if (skip)
            {
                Console.WriteLine("Turn Skipped");
                skip = false;
                currentcard = $"{ccparts[0]},{ccparts[1]},{ccparts[2]}";
                if (!aiplayer) Console.ReadKey();
                return;
            }
            if (int.Parse(ccparts[2]) > 0) { Console.WriteLine($"Current Draw cards = {int.Parse(ccparts[2])}"); }
            List<string> useablecards = ActionOptions(player);
            PlayAction(useablecards, int.Parse(ccparts[2]), player);
        }

        public static string PickColor()
        {
            string input = "";
            if (!aiplayer)
            {
                Console.WriteLine("Select a color\n1: Red\n2: Yellow\n3: Green\n4: Blue");
                input = Console.ReadLine();
            }
            else
            {
                input = random.Next(1, 5).ToString();
            }
            if (int.TryParse(input, out int color))
            {
                string output = $"{color - 1}";
                return output;
            }
            else
            {
                Console.WriteLine("Invalid input, try again");
                return PickColor();
            }
        }

        public static void Score()
        {
            for (int i = 1; i <= numberofplayers; i++)
            {
                int cardleft = playerhand[i].Count;
                if (cardleft == 0) { Console.WriteLine($"Player {i} Finnished!"); }
                else if (cardleft == 1)
                {
                    Console.Write("\u001b[1m");
                    Console.Write($"Player {i} ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("U");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("N");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("O");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("!");
                    Console.Write("\u001b[0m");
                }
                else { Console.WriteLine($"Player {i} Cards left: {cardleft}"); }
            }
        }

        public static void EndGame(int player)
        {
            if (playerhand[player].Count == 0) { winner.Add(player); }
            if (numberofplayers == winner.Count + 1)
            {
                foreach (List<string> a in playerhand) { if (a.Count > 0) { winner.Add(playerhand.IndexOf(a)); break; } }
                Console.Clear();
                int place = 0;
                foreach (int wplayer in winner)
                {
                    place++;
                    Console.WriteLine($"Player {wplayer} places: {place}");
                }
                Environment.Exit(0);
            }
        }
    }
}