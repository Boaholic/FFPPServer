using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using log4net;

//Create conversation and put conversation on outgoing queue
//dictionary mapping of conversation ID to conversation
namespace FFPPServer
{
    class DataProcessor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataProcessor));

        public MessageQueue _incomingMessages { get; set; }
        public  MessageQueue _outgoingMessages { get; set; }
        private bool keepProcessing = false;

        public void Enqueue(Message m)
        {
            if (m != null)
            {
                Log.Debug("Enqueue message = " + m);
                _outgoingMessages.Enqueue(m);
            }
        }

        public Message Dequeue()
        {
            Message result = null;

            if (_incomingMessages.Count > 0)
                result = _incomingMessages.Dequeue();

            if (result != null)
                Log.Debug("Dequeue message = " + result);
            return result;
        }

        public void Start()
        {
            keepProcessing = true;
        }

        public void Process()
        {
            while (keepProcessing)
            {
                Message _targetMessage = Dequeue();
                if (_targetMessage != null)
                {
                    Message _response = null;
                    switch (_targetMessage.thisMessageType)
                    {
                        case MessageType.JOIN:
                            {
                                // run add player command
                                _response = new Message(MessageType.ACK, "Adding to game");
                                Enqueue(_response);
                                break;
                            }
                        case MessageType.HB:
                            {

                                break;
                            }
                        case MessageType.CHAT:
                            {
                                //update chat form
                                break;
                            }
                    }

                } else
                {
                    Thread.Sleep(10);
                }
            }
                    
        }

        //process data here
    }
}
