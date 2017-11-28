using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FFPPServer
{
    /// <summary>
    /// Base Command Class
    /// 
    /// This is the abstract Command class prescribed by the command pattern
    /// </summary>
    public class Conversation
    {
        public LobbyServer Server { get; set; }
        public Message CurrentMessage { get; set; }
        public Message Response { get; set; }
        public int MessageID { get; set; } 
        public int ConversationID { get; set; }

        public int Timeout { get; set; }
        private int Attempts = 0;
        public int MaxAttempts { get; set; }

       public void WaitforResponse(int timeout)
        {
            while(MessageID == CurrentMessage.MessageID)
            {
                timeout -= 10;
                if(timeout == 0)
                {
                    ResendResponse();
                    Attempts++;
                }
            }
        }

        public bool CreateResponse(MessageType messageType, string MessageBody)
        {
            Message response = new Message(messageType, MessageBody);
            Communicator.Instance.Enqueue(Communicator.Instance.OutgoingQueue, response);
            return true;
        }

        public void ResendResponse() {
            if(Attempts >= MaxAttempts)
            {
                //Show some error here
                return;
            }

            //rend response 
            return;
        }
    }
}
