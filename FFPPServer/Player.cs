using log4net;
using System.Net;
using System.Net.Sockets;
using FFPPCommunication;
using System;
using System.Collections.Generic;

namespace FFPPServer
{
    public class Player
    { 
        // Player's name.
        public string Name { get; set; }
        public Guid PlayerID { get; set; }
        public List<GameServer> JoinedGames { get; set; }
        public IPEndPoint ClientAddress { get; set; }
        public bool IsReady { get; set; }
        // Locations of the players' ships.
        public int[,] ShipSet { get; set; }
        // [true] revieled / [false] unrevieled.
        public bool[,] RevealedCells { get; set; }
        // Last revieled cells.
        public int[] LastRevieledCells { get; set; }
        // Ships cells left.
        public int[] ShipLeftCells { get; set; }

        // Hits count.
        public int Hits { get; set; }
        // Misses count.
        public int Misses { get; set; }
        // Hit ratio.
        public double HitRatio { get; set; }
        // Unrevealed cells count.
        public int UnrevealedCells { get; set; }
        // Ships cells count.
        public int ShipCells { get; set; }
        // Ships left count.
        public int ShipsLeft { get; set; }

        // Battle log content.
        public string BattleLog { get; set; }

        public Player()
        {
            ShipLeftCells = new int[] { 2, 3, 3, 4, 5 };
            Hits = 0;
            Misses = 0;
            HitRatio = 0;
            UnrevealedCells = 100;
            ShipCells = 17;
            ShipsLeft = 5;
            BattleLog = "";
            ShipSet = new int[10, 10];
            RevealedCells = new bool[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    ShipSet[i, j] = -1;
                    RevealedCells[i, j] = false;
                }
            }

            LastRevieledCells = new int[2];
            LastRevieledCells[0] = -1;
            LastRevieledCells[1] = -1;

            JoinedGames = new List<GameServer>();
        }
        public bool hasJoinedGame
        {
            get
            {
                return hasJoinedGame;
            }
            set
            {
                hasJoinedGame = value;
            }
        }
        public bool playerHasRequestedJoin
        {
            get
            {
                return playerHasRequestedJoin;
            }
            set
            {
                playerHasRequestedJoin = value;
            }
        }
        public bool playerHasRequestedLeave
        {
            get
            {
                return playerHasRequestedLeave;
            }
            set
            {
                playerHasRequestedLeave = value;
            }
        }
        // public ServerMessage SendIsReady()
        //{
        //create an isReady message
        //send an isReady message
        // }
    }
}
