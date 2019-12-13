using BattleShipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipLiteLibrary
{
    public static class GameLogic
    {
        public static void InitializeGrid(PlayerModel model)
        {
            List<string> letters = new List<string>
            {
                "A",
                "B",
                "C",
                "D",
                "E"
            };

            List<int> numbers = new List<int>
            {
                1,
                2,
                3,
                4,
                5
            };

            foreach (string letter in letters)
            {
                foreach (int number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }
            }
        }

        private static void AddGridSpot(PlayerModel model, string letter, int number)
        {
            GridSpotModel spot = new GridSpotModel
            {
                SpotLetter = letter,
                SpotNumber = number,
                Status = GridSpotStatus.Empty
            };

            model.ShotGrid.Add(spot);
        }

        public static bool PlayerStillActive(PlayerModel player)
        {
            bool isActive = false;

            foreach (var ship in player.ShipLocations)
            {
                if(ship.Status != GridSpotStatus.Sunk)
                {
                    isActive = true;
                }
            }

            return isActive;
        }

        public static bool PlaceShip(PlayerModel model, string location)
        {
            bool output = false;
            (string row, int column) = SplitShotIntoRowAndColumn(location);

            bool isValidLocation = ValidateGridLocation(model, row, column);
            bool isSpotOpen = ValidateShipLocation(model, row, column);

            if(isValidLocation && isSpotOpen)
            {
                model.ShipLocations.Add(new GridSpotModel
                {
                    SpotLetter = row,
                    SpotNumber = column,
                    Status = GridSpotStatus.Ship
                });
                output = true;
            }

            return output;
        }

        private static bool ValidateShipLocation(PlayerModel model, string row, int column)
        {
            bool isValid = true;

            foreach (var ship in model.ShipLocations)
            {
                if (ship.SpotLetter.ToUpper() == row.ToUpper() && ship.SpotNumber == column)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        private static bool ValidateGridLocation(PlayerModel model, string row, int column)
        {
            bool isValid = false;

            foreach (var ship in model.ShotGrid)
            {
                if (ship.SpotLetter.ToUpper() == row.ToUpper() && ship.SpotNumber == column)
                {
                    isValid = true;
                }
            }

            return isValid;
        }

        public static int GetShotCount(PlayerModel player)
        {
            int shotCount = 0;

            foreach (var shot in player.ShotGrid)
            {
                if(shot.Status != GridSpotStatus.Empty)
                {
                    shotCount++;
                }
            }

            return shotCount;
        }

        public static (string row, int column) SplitShotIntoRowAndColumn(string shot)
        {
            string row = "";
            int column = 0;

            if(shot.Length != 2)
            {
                throw new ArgumentException("This was an invalid location", shot);
            }

            char[] shotArray = shot.ToArray();

            row = shotArray[0].ToString();
            column = int.Parse(shotArray[1].ToString());

            return (row, column);
        }

        public static bool ValidateShot(PlayerModel player, string row, int column)
        {
            bool isValid = false;

            foreach (var gridSpot in player.ShotGrid)
            {
                if (gridSpot.SpotLetter.ToUpper() == row.ToUpper() && gridSpot.SpotNumber == column)
                {
                    if(gridSpot.Status == GridSpotStatus.Empty)
                    {
                        isValid = true;
                    }
                }
            }

            return isValid;
        }

        public static bool IdentifyShotResult(PlayerModel opponent, string row, int column)
        {
            bool isValid = false;

            foreach (var ship in opponent.ShipLocations)
            {
                if (ship.SpotLetter.ToUpper() == row.ToUpper() && ship.SpotNumber == column)
                {
                    isValid = true;
                    ship.Status = GridSpotStatus.Sunk;
                }
            }

            return isValid;
        }

        public static void MarkShotResult(PlayerModel player, string row, int column, bool isAHit)
        {
            foreach (var gridSpot in player.ShotGrid)
            {
                if (gridSpot.SpotLetter.ToUpper() == row.ToUpper() && gridSpot.SpotNumber == column)
                {
                    if (isAHit)
                    {
                        gridSpot.Status = GridSpotStatus.Hit;
                    }
                    else
                    {
                        gridSpot.Status = GridSpotStatus.Miss;
                    }
                }
            }
        }
    }
}
