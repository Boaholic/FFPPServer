using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

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
            log.Info("Server Started!");
            Console.ReadLine();
        }
    }
}
