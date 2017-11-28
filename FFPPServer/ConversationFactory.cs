
using FFPPServer.Conversations;

namespace FFPPServer
{
    /// <summary>
    /// CommandFactory
    /// 
    /// Creates standard commands, but can be specialized to create custom commands.  This class is the base
    /// class in a factory method pattern.
    /// </summary>
    public class ConversationFactory
    {
        public LobbyServer Server { get; set; }
        private static ConversationFactory _instance;
        private static readonly object MyLock = new object();
        private ConversationFactory() { }

        public static ConversationFactory Instance
        {
            get
            {
                lock (MyLock)
                {
                    if (_instance == null)
                        _instance = new ConversationFactory();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Create -- a factory method for standard commands 
        /// 
        /// This method can be overridden to generate different or custom commands.
        /// </summary>
        /// <param name="commandType">type of command to Create:
        ///             New
        ///             AddTree
        ///             AddLine
        ///             AddDraw
        ///             Remove
        ///             Select
        ///             Deselect
        ///             Load
        ///             Save</param>
        /// <param name="commandParameters">An array of optional parametesr whose sementics depedent on the command type
        ///     For new, no additional parameters needed
        ///     For add, 
        ///         [0]: Type       reference type for assembly containing the tree type resource
        ///         [1]: string     tree type -- a fully qualified resource name
        ///         [2]: Point      center location for the tree, defaut = top left corner
        ///         [3]: float      scale factor</param>
        ///     For remove, no additional parameters needed
        ///     For select,
        ///         [0]: Point      Location at which a tree could be selected
        ///     For deselect, no additional parameters needed
        ///     For load,
        ///         [0]: string     filename of file to load from  
        ///     For save,
        ///         [0]: string     filename of file to save to  
        /// <returns></returns>
        public virtual Conversation CreateConversation(Message message)
        {

            Conversation conversation = null;
            switch (message.thisMessageType)
            {
                case MessageType.JOIN:
                    conversation = new JoinConversation
                    {
                        Server = Server,
                        CurrentMessage = message,
                        MaxAttempts = 3,
                        Timeout = 1000,
                    };
                    break;
                case MessageType.CHAT:
                    conversation = new ChatConversation
                    {
                        Server = Server,
                        CurrentMessage = message,
                        MaxAttempts = 3,
                        Timeout = 1000
                    };
                    break;
            }
            return conversation;
        }
    }
}

