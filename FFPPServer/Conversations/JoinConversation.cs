using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFPPServer.Conversations
{
    class JoinConversation : Conversation
    {

        public bool AddPlayer()
        {
            CreateResponse(MessageType.ACK, "Adding Player to game");
            return true;
        }
    }
}
