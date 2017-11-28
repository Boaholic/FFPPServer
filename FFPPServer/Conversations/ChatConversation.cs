using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFPPServer.Conversations
{
    class ChatConversation : Conversation
    {

        public bool BroadCastChat()
        {
            CreateResponse(MessageType.ACK, "Sending Message");
            return true;
        }
    }
}
