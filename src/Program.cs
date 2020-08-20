using System;
using System.Collections.Generic;

namespace C__projects
{
    class Program
    {
        static void Main(string[] args)
        {
            // Input number of payers
            List<Player> players = new List<Player>();
            Console.WriteLine("How many players?");
            int howManyPlayers = Convert.ToInt32(Console.ReadLine());

            // Input name of players
            for (int i = 0; i < howManyPlayers; i++)
            {
                Console.WriteLine("Name the player");
                Player player = new Player() {Name = Console.ReadLine() };
                players.Add(player);
            }
        }
    }
}
