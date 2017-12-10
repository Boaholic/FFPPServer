using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFPPCommunication;
using log4net;
using System.Net;

namespace FFPPServer
{
    public class DataProccessor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DataProccessor));
        public LobbyServer Lobby { get; set; }
        private List<GameServer> ActiveGames = new List<GameServer>();
        private bool keepProccessing;

        public DataProccessor()
        {
            keepProccessing = true;
        }
        public void Start()
        {
            log.Info("Starting Data Proccessor");
            Thread LobbyCommunicator = new Thread(new ThreadStart(Lobby.Communicator.Listen));
            LobbyCommunicator.Start();

            while (keepProccessing)
            {
                ProccessRequests();
            }
        }

        private void ProccessRequests()
        {
            Message lobbyRequest = Lobby.Communicator.Dequeue();

            if (lobbyRequest != null)
            {
                string[] props = ParseBody(lobbyRequest);
                log.Info($"New Request: Action: {props[0]}");
                bool success = DoAction(lobbyRequest.fromAddress, props);
            }
        }

        private string[] ParseBody(Message request)
        {
            return request.messageBody.Split('|');
        }

        private bool DoAction(IPEndPoint returnAddress, string[] props)
        {
            string action = props[0];
            bool result = false;
            bool playerAdded;
            bool playerRemoved;
            string responseBody;
            Message response;
            Player player;
            GameServer game;
            action = action.ToUpper();
            switch (action)
            {
                case "CONNECT":
                    Player newPlayer = new Player() { Name = props[1], PlayerID = new Guid(), ClientAddress = new IPEndPoint(IPAddress.Parse(props[2]), Int32.Parse(props[3]))};
                    playerAdded = Lobby.NewLobbyPlayer(newPlayer);
                    responseBody = $"PlayerConnected|{playerAdded}|OpenGames|";

                    responseBody = AddOpenGamesToResponse(responseBody);

                    response = new Message(Message.messageType.ACK, responseBody);
                    result = Lobby.Communicator.Send(response, returnAddress);

                    Lobby.BroadcastLog();
                    break;
                case "JOINGAME":
                    player = Lobby.TotalAssociatedPlayers.Find(person => person.Name == props[1]);
                    game = Lobby.GamesInLobby.Find(selectedGame => selectedGame.GameHandle == props[2]);
                    playerAdded = Lobby.AddPlayerToGame(player, game);

                    if(playerAdded)
                    {
                        player.JoinedGames.Add(game);
                    }

                    responseBody = $"PlayerAdded|{playerAdded}|OpenGames|";
                    responseBody = AddOpenGamesToResponse(responseBody);
                    responseBody += "|JoinedGames|";

                    responseBody = AddJoinedGamesToResponse(responseBody, player);

                    response = new Message(Message.messageType.ACK, responseBody);
                    result = Lobby.Communicator.Send(response, returnAddress);

                    Lobby.BroadcastLog();
                    break;
                case "LEAVEGAME":
                    player = Lobby.TotalAssociatedPlayers.Find(person => person.Name == props[1]);
                    game = Lobby.GamesInLobby.Find(selectedGame => selectedGame.GameHandle == props[2]);
                    playerRemoved = Lobby.RemovePlayerFromGame(player, game);

                    if (playerRemoved)
                    {
                        player.JoinedGames.Remove(game);
                    }

                    responseBody = $"PlayerAdded|{playerRemoved}|OpenGames|";
                    responseBody = AddOpenGamesToResponse(responseBody);
                    responseBody += "|JoinedGames|";

                    responseBody = AddJoinedGamesToResponse(responseBody, player);

                    response = new Message(Message.messageType.ACK, responseBody);
                    result = Lobby.Communicator.Send(response, returnAddress);

                    Lobby.BroadcastLog();
                    break;
                case "CREATEGAME":
                    player = Lobby.TotalAssociatedPlayers.Find(person => person.Name == props[1]);
                    game = new GameServer(props[2]);
                    

                    playerAdded = Lobby.PlayerCreateGame(player, game);

                    if (playerAdded)
                    {
                        player.JoinedGames.Add(game);
                    }

                    responseBody = $"GameCreated|{playerAdded}|OpenGames|";
                    responseBody = AddOpenGamesToResponse(responseBody);
                    responseBody += "|JoinedGames|";

                    responseBody = AddJoinedGamesToResponse(responseBody, player);

                    response = new Message(Message.messageType.ACK, responseBody);
                    result = Lobby.Communicator.Send(response, returnAddress);

                    Lobby.BroadcastLog();
                    break;
            }

            return result;
        }

        private string AddOpenGamesToResponse(string responseBody)
        {
            if(Lobby.GamesInLobby.Count == 0)
            {
                return responseBody;
            }

            GameServer LastOpenGame = Lobby.GamesInLobby.Last();
            foreach (GameServer Game in Lobby.GamesInLobby)
            {
                if (Game.Equals(LastOpenGame))
                {
                    responseBody += $"{Game.GameHandle} - {Game.JoinedPlayers.Count}/2";
                }
                else
                {
                    responseBody += $"{Game.GameHandle} - {Game.JoinedPlayers.Count}/2,";
                }

            }
            return responseBody;
        }
        private string AddJoinedGamesToResponse(string responseBody, Player player)
        {
            if(player.JoinedGames.Count == 0)
            {
                return responseBody;
            }

            GameServer LastJoinedGame = player.JoinedGames.Last();
            foreach (GameServer Game in player.JoinedGames)
            {
                if (Game.Equals(LastJoinedGame))
                {
                    responseBody += $"{Game.GameHandle} - {Game.JoinedPlayers.Count}/2";
                }
                else
                {
                    responseBody += $"{Game.GameHandle} - {Game.JoinedPlayers.Count}/2,";
                }

            }
            return responseBody;
        }

    }
}
