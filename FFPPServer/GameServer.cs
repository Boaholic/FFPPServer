using System;
using System.Collections.Generic;
using System.Text;
using FFPPCommunication;

namespace FFPPServer
{

    public class GameServer
    {
        //https://www.codeproject.com/Articles/140911/log-net-Tutorial
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
                typeof(GameServer)
            );

        public Communicator GameCommunicator = new Communicator();

        public bool isActive;
        public bool isOpen;
        public int maxPlayers;
        public bool isFull;

        public Guid GameID { get; set; }

        // Difficulty of the game, 1 for low, 10 for high
        public int Difficulty = 8;

        // Ship lengths.
        public int[] shipLengths = new int[5] { 2, 3, 3, 4, 5 };

        //Stuff for logging
        public char[] letterLabels = new char[10] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
        public string[] numberLables = new string[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        public string[] shipLabels = new string[5] { "Patrol Boat,", "Submarine,", "Destroyer,", "Battleship,", "Aircraft Carrier," };

        public List<Player> JoinedPlayers = new List<Player>();
        //true if using this game mode
        public bool gameMode { get; set; }
        // true == player-one's move/ false == player-two's move.
        public bool playerSwitch { get; set; }
        public int roundCount { get; set; }
        public Player player1 { get; set; }
        public Player player2 { get; set; }

        public GameServer()
        {
            GameID = Guid.NewGuid();
            isActive = false;
            isOpen = true;
        }
        public bool playerJoinGame(Player newPlayer)
        {
            foreach (Player p in JoinedPlayers)
            {
                if (newPlayer == p)
                {
                    log.Info("Player made duplicate request to join game server.");
                    return false; //player is already in game
                }
            }
            JoinedPlayers.Add(newPlayer);
            log.Info("New Player Joined.");
            return true;
        }

        public bool AllReady()
        {
            List<Player> ReadyPlayers = JoinedPlayers.FindAll(player => player.IsReady == true);
            if(ReadyPlayers.Count == JoinedPlayers.Count)
            {
                return true;
            }
            return false;
        }

        public void StartGame()
        {
            isActive = true;
            isOpen = false;
        }

        public bool CanPlaceShip(int currentShip, int cellX, int cellY, bool isHorizontal, int[,] shipSet)
        {
            // index of the most upper-left cell
            if (cellX < 0 || cellY < 0)
            {
                return false;
            }

            if (isHorizontal)
            {
                if (cellX + shipLengths[currentShip] - 1 <= 9)
                {
                    // Searching for an invalid layout on the grid.
                    for (int i = Math.Max(0, cellX - 1); i <= Math.Min(9, cellX + shipLengths[currentShip]); i++)
                    {
                        for (int j = Math.Max(0, cellY - 1); j <= Math.Min(9, cellY + 1); j++)
                        {
                            if (shipSet[i, j] != -1)
                            {
                                // Invalid layout found.
                                return false;
                            }
                        }
                    }

                    // Invalid layout not found.
                    return true;
                }
                else
                {
                    // Out of the bounds of the grid.
                    return false;
                }
            }
            else
            {
                // Vertical validation.
                if (cellY + shipLengths[currentShip] - 1 <= 9)
                {
                    // Searching for an invalid layout on the grid.
                    for (int i = Math.Max(0, cellX - 1); i <= Math.Min(9, cellX + 1); i++)
                    {
                        for (int j = Math.Max(0, cellY - 1); j <= Math.Min(9, cellY + shipLengths[currentShip]); j++)
                        {
                            if (shipSet[i, j] != -1)
                            {
                                // Invalid layout found.
                                return false;
                            }
                        }
                    }

                    // Invalid layout not found.
                    return true;
                }
                else
                {
                    // Out of the bounds of the grid.
                    return false;
                }
            }
        }

        // Add ship to shipset
        public void DeployShip(int currentShip, int cellX, int cellY, bool isHorizontal, int[,] shipSet)
        {
            if (isHorizontal)
            {
                for (int i = 0; i < shipLengths[currentShip]; i++)
                {
                    // Deploy into a ship set.
                    shipSet[cellX + i, cellY] = currentShip;
                }
            }
            else
            {
                for (int i = 0; i < shipLengths[currentShip]; i++)
                {
                    // Deploy into a ship set.
                    shipSet[cellX, cellY + i] = currentShip;
                }
            }

        }

        // Delete a ship from the ship set
        public void DeleteShip(int currentShip, int[,] shipSet)
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (shipSet[x, y] == currentShip)
                    {
                        shipSet[x, y] = -1;
                    }
                }
            }
        }

        // Perform an attack of a player on a player at a given cell.
        // [true] if game is over and the attacker won / [false] if not.
        public bool Attack(int cellX, int cellY, Player attacker, Player attacked)
        {
            string attackerLogNote = "--> " + "Round: " + String.Format("{0:000}", roundCount) + attacker.Name + ", Firing on "
                + attacked.Name + " at [ " + letterLabels[cellX] + "," + numberLables[cellY] + " ]. ";
            string attackedLogNote = "<-- " + "Round:" + String.Format("{0:000}", roundCount) + "  You, have been fired upon by: " + attacker.Name +
                " at [ " + letterLabels[cellX] + "," + numberLables[cellY] + " ]. ";

            // Mark the cell as revealed and update counts
            attacked.RevealedCells[cellX, cellY] = true;
            attacked.UnrevealedCells--;

            attacked.LastRevieledCells[0] = cellX;
            attacked.LastRevieledCells[1] = cellY;

            // Attack hit
            if (attacked.ShipSet[cellX, cellY] != -1)
            {
                //Play hit sound
                // Decrease the amount of ship cells left.
                attacked.ShipCells--;
                // Increase the count of attacker's hits.
                attacker.Hits++;
                // Recalculate the attacker's hit ratio.
                attacker.HitRatio = Convert.ToDouble(attacker.Hits) / Convert.ToDouble(roundCount);

                // Decrease the amount of cells left for the ship that has been hit.
                attacked.ShipLeftCells[attacked.ShipSet[cellX, cellY]]--;

                attackedLogNote = attackedLogNote + shipLabels[attacked.ShipSet[cellX, cellY]] + " has been hit! "
                    + attacked.ShipLeftCells[attacked.ShipSet[cellX, cellY]].ToString() + " hits remaining. ";
                attackerLogNote = attackerLogNote + "You've hit " + attacked.Name + "'s ship! ";
                // sunken ship
                if (attacked.ShipLeftCells[attacked.ShipSet[cellX, cellY]] == 0)
                {

                    attacked.ShipsLeft--;

                    attackedLogNote = attackedLogNote + "Ship SUNK!. You have " + attacked.ShipsLeft.ToString() + " more ships left. ";
                    attackerLogNote = attackerLogNote + "You have sunk: " + attacked.Name + "'s" +
                        shipLabels[attacked.ShipSet[cellX, cellY]] + ". They have " + attacked.ShipsLeft.ToString() + " ships remaining. ";


                    int extraRevealedCells = 0;

                    // Reveal neighbouring cells of the sunken ship.
                    for (int x = 0; x < 10; x++)
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            if (attacked.ShipSet[x, y] == attacked.ShipSet[cellX, cellY])
                            {
                                try
                                {
                                    if (!attacked.RevealedCells[x - 1, y - 1])
                                    {
                                        attacked.RevealedCells[x - 1, y - 1] = true;
                                        extraRevealedCells++;
                                    }
                                }
                                catch { };

                                try
                                {
                                    if (!attacked.RevealedCells[x - 1, y])
                                    {
                                        attacked.RevealedCells[x - 1, y] = true;
                                        extraRevealedCells++;
                                    }
                                }
                                catch { };

                                try
                                {
                                    if (!attacked.RevealedCells[x - 1, y + 1])
                                    {
                                        attacked.RevealedCells[x - 1, y + 1] = true;
                                        extraRevealedCells++;
                                    }
                                }
                                catch { };

                                try
                                {
                                    if (!attacked.RevealedCells[x, y - 1])
                                    {
                                        attacked.RevealedCells[x, y - 1] = true;
                                        extraRevealedCells++;
                                    }
                                }
                                catch { };

                                try
                                {
                                    if (!attacked.RevealedCells[x, y + 1])
                                    {
                                        attacked.RevealedCells[x, y + 1] = true;
                                        extraRevealedCells++;
                                    }
                                }
                                catch { };

                                try
                                {
                                    if (!attacked.RevealedCells[x + 1, y - 1])
                                    {
                                        attacked.RevealedCells[x + 1, y - 1] = true;
                                        extraRevealedCells++;
                                    }
                                }
                                catch { };

                                try
                                {
                                    if (!attacked.RevealedCells[x + 1, y])
                                    {
                                        attacked.RevealedCells[x + 1, y] = true;
                                        extraRevealedCells++;
                                    }
                                }
                                catch { };

                                try
                                {
                                    if (!attacked.RevealedCells[x + 1, y + 1])
                                    {
                                        attacked.RevealedCells[x + 1, y + 1] = true;
                                        extraRevealedCells++;
                                    }
                                }
                                catch { };
                            }
                        }
                    }

                    // Decrease the number of unrevealed cells.
                    attacked.UnrevealedCells -= extraRevealedCells;

                    // Is the game over?
                    if (attacked.ShipsLeft == 0)
                    {
                        attackerLogNote = attackerLogNote + attacker.Name.ToString() + " won the battle!";
                        attacked.BattleLog = attacked.BattleLog + attackedLogNote + "\n";
                        attacker.BattleLog = attacker.BattleLog + attackerLogNote + "\n";
                        return true;
                    }
                    else
                    {
                        // Else return a false, some ships are left
                        attacked.BattleLog = attacked.BattleLog + attackedLogNote + "\n";
                        attacker.BattleLog = attacker.BattleLog + attackerLogNote + "\n";
                        return false;
                    }
                }
                else
                {
                    //There are some ship cells left in this ship, so that Game dont end
                    attacked.BattleLog = attacked.BattleLog + attackedLogNote + "\n";
                    attacker.BattleLog = attacker.BattleLog + attackerLogNote + "\n";
                    return false;
                }
            }
            else
            {
                // The attack is not a hit.
                attackedLogNote = attackedLogNote + " MISS!.";
                attackerLogNote = attackerLogNote + " MISS!.";
                attacker.Misses++;
                attacker.HitRatio = Convert.ToDouble(attacker.Hits) / Convert.ToDouble(roundCount);

                //return miss
                attacked.BattleLog = attacked.BattleLog + attackedLogNote + "\n";
                attacker.BattleLog = attacker.BattleLog + attackerLogNote + "\n";
                return false;
            }

        }
    }
}
