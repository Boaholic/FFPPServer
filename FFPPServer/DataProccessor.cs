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
            action = action.ToUpper();
            switch (action)
            {
                case "CONNECT":
                    Player newPlayer = new Player() { Name = props[1], PlayerID = new Guid() };
                    bool playerAdded = Lobby.NewLobbyPlayer(newPlayer);
                    string responseBody = $"PlayerAdded|{playerAdded}|OpenGames|";
                    GameServer LastOpenGame = Lobby.GamesInLobby.Last();
                    foreach(GameServer Game in Lobby.GamesInLobby)
                    {   
                        if(Game.Equals(LastOpenGame))
                        {
                            responseBody += $"{Game.GameID}";
                        }
                        else
                        {
                            responseBody += $"{Game.GameID},";
                        }
                       
                    }

                    Message response = new Message(Message.messageType.ACK, responseBody);
                    result = Lobby.Communicator.Send(response, returnAddress);
                    break;
            }

            return result;
        }
       
    }
}
