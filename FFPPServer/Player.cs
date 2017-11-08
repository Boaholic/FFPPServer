using log4net;
using System.Net;
using System.Net.Sockets;

namespace FFPPServer
{
    public class Player
    {
        public string playerName { get; set; }
        public static readonly ILog Logger = LogManager.GetLogger(typeof(Message));
        public string A_Number { get; set; }
        public IPAddress _ServerAddress { get; set; }
        public UdpClient MyUdpClient { get; set; }

        public IPEndPoint myEndPoint;
        public IPEndPoint _ServerEndPoint;
        public static System.Timers.Timer Controller;
        public int Port { get; set; }
        // System.Timers.TImer, db heartbeat, in score , s answer 

        public int Score { get; set; }

        public Player(Message initialClientMessage)
        {
            //extract the player name
            //set the value of variable playerName
            //to the input value
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
