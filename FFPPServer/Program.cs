using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFPPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set up server
            LobbyServer _server = new LobbyServer();
            ConversationFactory.Instance.Server = _server;

            //Set up message queues
            MessageQueue _incomingMessages = new MessageQueue();
            MessageQueue _outgoingMessages = new MessageQueue();
            Communicator.Instance.IncomingQueue = _incomingMessages;
            Communicator.Instance.OutgoingQueue = _outgoingMessages;

            //Setup and start Threads 
            Thread Listener = new Thread(new ThreadStart(Communicator.Instance.Listen));
            Thread Sender = new Thread(new ThreadStart(Communicator.Instance.Send));
            Thread Processor = new Thread(new ThreadStart(DataProcessor.Instance.Start));

            Listener.Start();
            Sender.Start();
            Processor.Start();
        }
    }
}
