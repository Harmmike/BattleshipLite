using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShipLiteLibrary;
using BattleShipLiteLibrary.Models;

namespace BattleShipLite
{
    class Program
    {
        public const double AppVersion = 1.0;

        static void Main(string[] args)
        {
            NewGame();

            Console.ReadLine();
        }

        private static void NewGame()
        {
            WelcomeMessage();

            PlayerModel activePlayer = CreatePlayer("Player 1");
            PlayerModel opponent = CreatePlayer("Player 2");

            PlayerModel winner = null;

            do
            {
                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, opponent);

                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                if (doesGameContinue)
                {
                    ////Temp variable swap.
                    //PlayerModel temp = opponent;
                    //opponent = activePlayer;
                    //activePlayer = temp;

                    //Use Tuple, my first Tuple attempt.
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else
                {
                    winner = activePlayer;
                }

            } while (winner == null);

            IdentifyWinner(winner);
        }

        private static void IdentifyWinner(PlayerModel winner)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine($"Congratulations {winner.Username}!  You are the winner!");
            Console.WriteLine($"{winner.Username} took {GameLogic.GetShotCount(winner)} shots to win!");
            Console.WriteLine();

            Console.ResetColor();

            Console.Write("Enter 1 to start a new game, enter 2 to exit. ");
            string choice = Console.ReadLine();

            if(int.Parse(choice) == 1)
            {
                Console.Clear();
                NewGame();
            }
            else if(int.Parse(choice) == 2)
            {
                Environment.Exit(0);
            }
        }

        private static void RecordPlayerShot(PlayerModel activePlayer, PlayerModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;

            do
            {
                string shot = AskForShot(activePlayer);

                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                    isValidShot = false;
                }

                if (!isValidShot)
                {
                    Console.WriteLine("Invalid shot location, please try again...");
                }

            } while (!isValidShot);

            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);

            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            DisplayShotResults(row, column, isAHit);
        }

        private static void DisplayShotResults(string row, int column, bool isAHit)
        {
            if (isAHit)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{row}{column} is a hit!");
                Console.ResetColor();
                Console.WriteLine("__________________________________");
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{row}{column} is a miss.");
                Console.ResetColor();
                Console.WriteLine("__________________________________");
                Console.WriteLine();
            }
        }

        private static string AskForShot(PlayerModel player)
        {
            string output = "";

            do
            {
                Console.WriteLine();
                Console.Write($"Please enter shot selection for {player.Username}: ");
                output = Console.ReadLine();
                Console.WriteLine();

            } while (output.Length != 2);

            return output;
        }

        private static void DisplayShotGrid(PlayerModel activePlayer)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            string currentRow = activePlayer.ShotGrid[0].SpotLetter; //Set the currentRow to the first letter in the grid.

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if(gridSpot.SpotLetter != currentRow) //First row *should* be "A", so if we're writing "B+" then it will skip a line and update the currentRow.
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }                

                if(gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter}{gridSpot.SpotNumber} ");
                }
                else if(gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if(gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                }
                else
                {
                    Console.Write(" ? "); //we should never reach this
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.ResetColor();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite!");
            Console.WriteLine($"Version: {AppVersion}");
            Console.WriteLine("Created by Michael Harmon");
            Console.WriteLine();
        }

        private static PlayerModel CreatePlayer(string playerTitle)
        {
            Console.Write($"Player information for {playerTitle}: ");
            Console.WriteLine();

            //Create player
            PlayerModel player = new PlayerModel();

            //Get player's name
            player.Username = GetPlayerName();

            //Create player's grid.
            GameLogic.InitializeGrid(player);

            //Ask for ship placement
            PlaceShips(player);

            //Clear
            Console.Clear();

            //return the model.
            return player;
        }

        private static string GetPlayerName()
        {
            Console.Write("Please enter your name: ");
            string name = Console.ReadLine();
            return name;
        }

        private static void PlaceShips(PlayerModel model)
        {
            do
            {
                Console.Write($"Where do you want to place ship number {model.ShipLocations.Count + 1}? ");

                string location = Console.ReadLine();

                bool isValidLocation = false;

                try
                {
                    isValidLocation = GameLogic.PlaceShip(model, location);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }

                if (!isValidLocation)
                {
                    Console.WriteLine($"{location} is not a valid location, please try again.");
                }

            } while (model.ShipLocations.Count < 5);
        }
    }
}
