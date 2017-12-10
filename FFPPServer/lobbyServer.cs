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

        string LobbyLog { get; set; }

        public LobbyServer()
        {
            TotalAssociatedPlayers = new List<Player>();
            GamesInLobby = new List<GameServer>();
            Communicator = new Communicator();
            LobbyLog = "LOBBYLOG|";

            //Create two new instances of lobbygame
            GameServer firstGame = new GameServer
            {
                GameHandle = "Lobby Generated Game 1"
            };
            GameServer secondGame = new GameServer
            {
                GameHandle = "Lobby Generated Game 2"
            };
            GamesInLobby.Add(firstGame);
            GamesInLobby.Add(secondGame);
        }

        public void BroadcastLog()
        {

            Message logMessage = new Message(Message.messageType.CHAT, LobbyLog);
            foreach(Player player in TotalAssociatedPlayers)
            {

                Communicator.Send(logMessage, player.ClientAddress);
            }
        }
        public bool NewLobbyPlayer(Player newPlayer)
        {
            if(TotalAssociatedPlayers.Count != 0)
            {
                TotalAssociatedPlayers.Add(newPlayer);
            }
            else
            {
                Player ExsistingPlayer = TotalAssociatedPlayers.Find(player => player.Name == newPlayer.Name);
                if(ExsistingPlayer != null)
                {
                    return false;
                }
                TotalAssociatedPlayers.Add(newPlayer);
                LobbyLog = $"LOBBYLOG|New Player {newPlayer.Name} has entered Lobby";
            }
            return true;
        }

        public bool AddPlayerToGame(Player newPlayer, GameServer SelectedGame)
        {
           
            if(SelectedGame.isFull)
            {
                return false;
            }
            else
            {
                Player ExsistingPlayer = SelectedGame.JoinedPlayers.Find(player => player.Name == newPlayer.Name);
                if (ExsistingPlayer != null)
                {
                    return false;
                }
                LobbyLog = $"LOBBYLOG|Player {newPlayer.Name} joined Game {SelectedGame.GameHandle}";

                SelectedGame.JoinedPlayers.Add(newPlayer);
                if(SelectedGame.JoinedPlayers.Count == 2)
                {
                    SelectedGame.isFull = true;
                }
            }
        
            return true;
        }

        public bool RemovePlayerFromGame(Player newPlayer, GameServer SelectedGame)
        {
            SelectedGame.JoinedPlayers.Remove(newPlayer);
            SelectedGame.isFull = false;

            LobbyLog = $"LOBBYLOG|Player {newPlayer.Name} left Game {SelectedGame.GameHandle}";
            return true;
        }

        public bool PlayerCreateGame(Player player, GameServer NewGame)
        {
            NewGame.JoinedPlayers.Add(player);
            GamesInLobby.Add(NewGame);

            LobbyLog = $"LOBBYLOG|Player {player.Name} created and joined Game {NewGame.GameHandle}";
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
