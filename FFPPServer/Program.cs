using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.Threading;

namespace FFPPServer
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
           // XmlConfigurator.Configure(new System.IO.FileInfo("./App.config"));   
            //BasicConfigurator.Configure();
            LobbyServer mainLobby = new LobbyServer();
            DataProccessor dataProccessor = new DataProccessor
            {
                Lobby = mainLobby
            };

            Thread ProccessorThread = new Thread(new ThreadStart(dataProccessor.Start));
            ProccessorThread.Start();

            log.Info("Server Started!");
            Console.ReadLine();
        }
    }
}
