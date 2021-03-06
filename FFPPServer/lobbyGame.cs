﻿using System;
using System.Collections.Generic;
using System.Text;
using FFPPCommunication;

namespace FFPPServer
{

    public class LobbyGame
    {
        //https://www.codeproject.com/Articles/140911/log-net-Tutorial
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
                typeof(LobbyGame)
            );
        public Player[] associatedPlayers;
        public bool isActive;
        public bool isOpen;
        public LobbyGame()
        {
            associatedPlayers = null;
            isActive = false;
            isOpen = true;
        }
        public void playerJoinGame(Player newPlayer)
        {
            foreach (Player p in associatedPlayers)
            {
                if (newPlayer == p)
                {
                    log.Info("Player made duplicate request to join server.");
                    return; //player is already in game
                }
            }
            associatedPlayers.SetValue(newPlayer, associatedPlayers.GetUpperBound(1) + 1);
            log.Info("New Player Joined.");
        }
    }
}
