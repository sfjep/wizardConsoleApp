using System;
using System.Collections.Generic;
using System.Media;

 
namespace C__projects
{
    public class Game
    {
        private int numberOfRounds;
        private List<Player> players;
        private int howManyPlayers;
        
        public Game()
        {
            players = new List<Player>();
            initializeGame();
            playGame();
        }

        public void initializeGame()
        {
            // Input number of players 
            Console.WriteLine("\nHow many players?");
            var input =  Console.ReadLine();
            howManyPlayers = Convert.ToInt32(input);
            numberOfRounds = 60 / howManyPlayers;

            assignPlayerNames();
        }

        public void assignPlayerNames()
        {
            for (int i = 0; i < howManyPlayers; i++)
            {
                Console.WriteLine($"\nName player {i+1}");
                string name = Console.ReadLine();
                Player player = new Player(name);
                players.Add(player);
            }
        }

        
        public void getRoundAndDealer(int turn)
        {
            // GET NAME OF DEALER
            var dealerNumber = turn % howManyPlayers;
            var playerDeals = players[dealerNumber].name;

            // PRINT TURN AND DEALER
            Console.WriteLine($"\n\nRound {turn + 1} - {playerDeals} deals\n");

        }
        
        public Dictionary<string, int> getTrickGuesses(int turn)
        {
            Dictionary<string, int> trickGuess = new Dictionary<string, int>();
            var seq = getRoundSequence(turn);
            
            var playerNumber = 1;
            var totalGuess = 0;
            int guess;
            int notAllowedGuess;

            foreach(int player in seq)
            {
                while(true)
                {   
                    Console.WriteLine($"\nHow many tricks does {players[player].name} want?");

                    // PROVIDE NUMBER OF TRICKS THE LAST PLAYER CANNOT GUESS
                    notAllowedGuess = lastPlayerCannotGuess(playerNumber, turn, totalGuess);

                    var input = Console.ReadLine();

                    // TRY CATCH ERRORS IN INPUT FORMAT
                    try
                    {
                        // IF INPUT IS EQUAL TO THE NOT ALLOWED GUESS, GET NEW INPUT 
                        input = guessNotAllowedOrLessThanZero(input, playerNumber, notAllowedGuess, turn);
                        guess = int.Parse(input);

                        // PLAY MINDGAMES ON DAVID
                        if(guess == 0 && players[player].name.ToLower() == "david")
                        {
                            playSound();
                        }

                        trickGuess.Add(players[player].name, guess);
                        
                        playerNumber ++;
                        
                        totalGuess += guess;
                    }
                    catch(ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    catch(FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    break;
                }
                
            }

            return trickGuess;
        }



        public Dictionary<string, int> getTrickResults(int turn)
        {
            Dictionary<string, int> trickResults = new Dictionary<string, int>();

            var seq = getRoundSequence(turn);

            restart:
            var trickCount = 0;
            var playerNumber = 1;

            foreach(int player in seq)
            {  
                while(true)
                {
                    Console.WriteLine($"\nHow many tricks did {players[player].name} get?");
                    
                    var input = Console.ReadLine();
                    
                    try
                    { 
                        // IF INPUT LESS THAN ZERO, GET NEW INPUT
                        input = resultLessThanZeroOrGreaterThanRoundNumber(input, playerNumber, turn);

                        var tricks = Convert.ToInt32(input);
                        trickResults.Add(players[player].name, tricks);
                        trickCount += tricks;

                        // IF TURN AND NUMBER OF TRICKS OBTAINED DON'T ADD UP - RESTART
                        if((turn+1) != trickCount && playerNumber == howManyPlayers)
                        {
                            Console.WriteLine("\nNumber of tricks does not add up, try again from the top!");
                            trickResults.Clear();
                            goto restart;
                        };
                    }
                    catch(ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    catch(FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    break;
                }
                playerNumber ++;
            }
            return trickResults;
        }

        private int lastPlayerCannotSay(int playerNumber, int turn, int totalGuess)
        {
            if(playerNumber == howManyPlayers && (turn + 1) > totalGuess)
            {   
                var notAllowedGuess = (turn + 1) - totalGuess;

                return notAllowedGuess;
            }
            else
            {
                var notAllowedGuess = -1;
                return notAllowedGuess;
            }
        }

        private string guessNotAllowedOrLessThanZero(string input, int playerNumber, int notAllowedGuess, int turn)
        {
            while(Convert.ToInt32(input) < 0 | (playerNumber == howManyPlayers && Convert.ToInt32(input) == notAllowedGuess) | (Convert.ToInt32(input) > (turn+1)))
            {
                if(Convert.ToInt32(input) < 0)
                {
                    Console.WriteLine($"\nYour guess cannot be negative, guess again");
                    input = Console.ReadLine();
                }
                else if(Convert.ToInt32(input) > (turn+1))
                {
                    Console.WriteLine($"\nYour guess is overly optimistic - try again");
                    input = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine($"\nYou cannot guess {notAllowedGuess}, guess again");
                    input = Console.ReadLine(); 
                }     
            }
            return input;
        }
        private string resultLessThanZeroOrGreaterThanRoundNumber(string input, int playerNumber, int turn)
        {
            while((Convert.ToInt32(input) < 0) | (Convert.ToInt32(input) > (turn + 1)))
            {
                if(Convert.ToInt32(input) < 0)
                {
                    Console.WriteLine($"\nResult cannot be negative, try again");
                    input = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine($"\nResult cannot be greater than round number, try again");
                    input = Console.ReadLine();                   
                }
            }
            return input;
        }

        private int lastPlayerCannotGuess(int playerNumber, int turn, int totalGuess)
        {
            if(playerNumber == howManyPlayers && (turn + 1) >= totalGuess)
            {   
                var notAllowedGuess = (turn + 1) - totalGuess;
                Console.WriteLine($"\nCannot guess {notAllowedGuess}");

                return notAllowedGuess;
            }
            else
            {
                var notAllowedGuess = -1;
                return notAllowedGuess;
            }
        }

        private List<int> getRoundSequence(int turn)
        {
            var startNumber = (turn + 1) % 4;

            List<int> seq = new List<int>();
            
            for(int i = startNumber; i <= howManyPlayers - 1; i ++)
            {
                seq.Add(i);
            }
            
            for(int i = 0; i < startNumber; i ++)
            {
                seq.Add(i);
            }

            return seq;
        }
               
        private void playGame()
        {
            for(int i = 0; i < numberOfRounds; i++)
            {
                playRound(i);
            }
        }

        private void playRound(int turn)
        {
            // GET ROUND NUMBER AND DEALER NAME
            getRoundAndDealer(turn);

            // INPUT GUESS
            var trickGuess = getTrickGuesses(turn);

            // INPUT RESULT
            var trickResults = getTrickResults(turn);

            // UPDATE SCORES
            getNewScores(trickGuess, trickResults);
        }

        private void getNewScores(Dictionary<string, int> trickGuess, Dictionary<string, int> trickResults)
        {
            Console.WriteLine("\nNew score board:");
            
            foreach(var player in players)
            {  
                var guess = trickGuess[player.name];
                var result = trickResults[player.name];
                var diff = Math.Abs(guess - result);
                
                if(diff == 0)
                {
                    player.score += guess * 10 + 20;
                }
                else
                {
                    player.score -= diff * 10;
                }

                Console.WriteLine($"{player.name}: {player.score}");                
            }           
        }

        private void playSound()
        {
            var chicken = new System.Media.SoundPlayer();
            var audio_dir = System.IO.Directory.GetParent(Environment.CurrentDirectory) + "\\audio\\chicken.wav";
            chicken.SoundLocation = audio_dir;
            chicken.PlaySync();
        }
    }
}