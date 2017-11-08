using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFPPServer
{
    public class lobbyServer
    {
        public clientPlayer[] totalAssociatedPlayers;
        public lobbyGame[] gamesInLobby;
        public void newLobbyPlayer(clientPlayer newPlayer)
        {
            foreach (clientPlayer p in totalAssociatedPlayers)
            {
                if (newPlayer == p)
                {
                    return; //The player already exists
                }
            }
            totalAssociatedPlayers.SetValue(newPlayer, totalAssociatedPlayers.GetUpperBound(1) + 1);
        }

        public lobbyServer()
        {
            totalAssociatedPlayers = null;
            //Create two new instances of lobbygame
            lobbyGame firstGame = new lobbyGame();
            lobbyGame secondGame = new lobbyGame();
            gamesInLobby.SetValue(firstGame, gamesInLobby.GetUpperBound(1) + 1);
            gamesInLobby.SetValue(secondGame, gamesInLobby.GetUpperBound(1) + 1);
        }
    }
}
