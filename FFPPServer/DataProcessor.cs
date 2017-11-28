using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using log4net;


namespace FFPPServer
{
    class DataProcessor
    {
        private static DataProcessor _instance;
        private static readonly object MyLock = new object();

        private static readonly ILog Log = LogManager.GetLogger(typeof(DataProcessor));

        public List<Conversation> RunningConversations { get; set; }
        private bool keepProcessing = false;

        private DataProcessor() { }

        public static DataProcessor Instance
        {
            get
            {
                lock (MyLock)
                {
                    if (_instance == null)
                        _instance = new DataProcessor();
                }
                return _instance;
            }
        }

        public void Enqueue(Message m)
        {
            if (m != null)
            {
                Log.Debug("Enqueue message = " + m);
                Communicator.Instance.OutgoingQueue.Enqueue(m);
            }
        }

        public Message Dequeue()
        {
            Message result = null;

            if (Communicator.Instance.IncomingQueue.Count > 0)
                result = Communicator.Instance.IncomingQueue.Dequeue();

            if (result != null)
                Log.Debug("Dequeue message = " + result);
            return result;
        }

        public void Start()
        {
            keepProcessing = true;
            Process();
        }

        public void Process()
        {
            if(RunningConversations == null)
            {
                RunningConversations = new List<Conversation>();
            }
            while (keepProcessing)
            {
                Message _targetMessage = Dequeue();
                if (_targetMessage != null)
                {
                    Conversation conversation = RunningConversations.Find(x => x.ConversationID == _targetMessage.ConversationID);
                    if(conversation == null)
                    {
                        Conversation _newConversation = ConversationFactory.Instance.CreateConversation(_targetMessage);
                        RunningConversations.Add(_newConversation);
                    } else
                    {
                        //update conversation
                    }
                    

                } else
                {
                    Thread.Sleep(10);
                }
            }
                    
        }

    }
}
