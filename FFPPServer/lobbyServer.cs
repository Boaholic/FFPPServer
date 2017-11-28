using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFPPCommunication;

namespace FFPPServer
{
    public class LobbyServer
    {
        public Player[] totalAssociatedPlayers = new Player[2];
        public LobbyGame[] gamesInLobby = new LobbyGame[2];
        public void newLobbyPlayer(Player newPlayer)
        {
            foreach (Player p in totalAssociatedPlayers)
            {
                if (newPlayer == p)
                {
                    return; //The player already exists
                }
            }
            totalAssociatedPlayers.SetValue(newPlayer, totalAssociatedPlayers.GetUpperBound(1) + 1);
        }

        public LobbyServer()
        {
            totalAssociatedPlayers = null;
            //Create two new instances of lobbygame
            LobbyGame firstGame = new LobbyGame();
            LobbyGame secondGame = new LobbyGame();
            gamesInLobby.SetValue(firstGame, 0);
            gamesInLobby.SetValue(secondGame, 1);
            
        }
    }
}
