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
        public List<Player> TotalAssociatedPlayers { get; set; }
        public List<GameServer> GamesInLobby { get; set; }
        public Communicator Communicator { get; set; }

        public LobbyServer()
        {
            TotalAssociatedPlayers = new List<Player>();
            GamesInLobby = new List<GameServer>();
            Communicator = new Communicator();

            //Create two new instances of lobbygame
            GameServer firstGame = new GameServer();
            GameServer secondGame = new GameServer();
            GamesInLobby.Add(firstGame);
            GamesInLobby.Add(secondGame);
        }
        public bool NewLobbyPlayer(Player newPlayer)
        {
            if(TotalAssociatedPlayers.Count != 0)
            {
                TotalAssociatedPlayers.Add(newPlayer);
            }
            else
            {
                foreach (Player p in TotalAssociatedPlayers)
                {
                    if (newPlayer == p)
                    {
                        return false; //The player already exists
                    }
                }
                TotalAssociatedPlayers.Add(newPlayer);  
            }
            return true;
        }

        public bool AddPlayerToGame(Player newPlayer, Guid GameIndex)
        {
            GameServer SelectedGame = GamesInLobby.Find(game => game.GameID == GameIndex);
            if(SelectedGame.isFull)
            {
                return false;
            }
            else
            {
                SelectedGame.JoinedPlayers.Add(newPlayer);
                if(SelectedGame.JoinedPlayers.Count == 2)
                {
                    SelectedGame.isFull = true;
                }
            }
        
            return true;
        }

        public bool RemovePlayerFromGame(Player newPlayer, Guid GameIndex)
        {
            GameServer SelectedGame = GamesInLobby.Find(game => game.GameID == GameIndex);
            SelectedGame.JoinedPlayers.Remove(newPlayer);
            SelectedGame.isFull = false;
            return true;
        }

        public bool ReadyUpPlayer(Player player, Guid GameIndex)
        {
            GameServer SelectedGame = GamesInLobby.Find(game => game.GameID == GameIndex);
            Player SelectedPlayer = SelectedGame.JoinedPlayers.Find(thisPlayer => thisPlayer.PlayerID == player.PlayerID);
            SelectedPlayer.IsReady = true;
            if(SelectedGame.AllReady())
            {
                StartGame(SelectedGame);
                return true;
            }
            return false;
        }

        public bool StartGame(GameServer SElectedGame)
        {
            return false;
        }

    }
}
