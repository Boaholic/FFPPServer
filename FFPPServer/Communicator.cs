using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using log4net;


namespace FFPPServer
{
    /// <summary>
    /// Summary description for Communicator.
    /// </summary>
    public class Communicator
    {

        #region Private and Protected Data Members
        private static Communicator _instance;
        private static readonly object MyLock = new object();

        private static readonly ILog Log = LogManager.GetLogger(typeof(Communicator));

        public int _localPort { get; set; }
        private static IPEndPoint _localEndPoint;
        private static UdpClient _udpClient;

        public MessageQueue IncomingQueue { get; set; }
        public MessageQueue OutgoingQueue { get; set; }
        private ReadWrite _readWrite = new ReadWrite();


        /// <summary>
        /// Default Constructor
        /// </summary>
        private Communicator() {
            Initialize();
        }

        public static Communicator Instance
        {
            get
            {
                lock (MyLock)
                {
                    if (_instance == null)
                        _instance = new Communicator();
                }
                return _instance;
            }
        }
        
        #endregion

        #region Constructors and Destructors

        public void Initialize()
        {
            Log.Debug("Initializing communicator");

            _localEndPoint = new IPEndPoint(IPAddress.Any, _localPort);
            _udpClient = new UdpClient(_localEndPoint);
            Log.Debug("Creating UdpClient with end point " + _localEndPoint);

            _localEndPoint = _udpClient.Client.LocalEndPoint as IPEndPoint;
            if (_localEndPoint != null)
            {
                _localPort = _localEndPoint.Port;
                Log.Info("Created Communicator's UdpClient, bound to " + _localEndPoint);


                Log.Debug("Done initializing communicator");
            }
           
        }

        #endregion

        #region Public Properties and Methods

        public bool keepSending => _udpClient != null;

        public Int32 LocalPort
        {
            get { return _localPort; }
            set { _localPort = value; }
        }

        public IPEndPoint LocalEndPoint => _localEndPoint;
        public void Enqueue(MessageQueue queue, Message m)
        {
            if (m != null)
            {
                Log.Debug("Enqueue message = " + m);
                queue.Enqueue(m);
            }
        }

        public Message Dequeue(MessageQueue queue)
        {
            Message result = null;

            if (queue.Count > 0)
                result = queue.Dequeue();

            if (result != null)
                Log.Debug("Dequeue message = " + result);
            return result;
        }

        public void Listen()
        {
            Log.Debug("Listening for messages");

            try
            {
                // Asyncronously listen for new messages
      
                    _udpClient.BeginReceive(new AsyncCallback(Receive), null);

            }
            catch (SocketException err)
            {
                if (err.SocketErrorCode != SocketError.TimedOut && err.SocketErrorCode != SocketError.ConnectionReset)
                    Log.ErrorFormat($"Socket error: {err.SocketErrorCode}, {err.Message}");
            }
            catch (Exception err)
            {
                Log.ErrorFormat($"Unexpected expection while receiving datagram: {err} ");
            }
            Log.Debug("Leaving Listening");
        }

        // recieved async message
        public void Receive(IAsyncResult res)
        {
            Log.InfoFormat(@"Packet available");
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            byte[] received = _udpClient.EndReceive(res, ref RemoteIpEndPoint);
            Log.Debug($"Bytes received: {FormatBytesForDisplay(received)}");

            _udpClient.BeginReceive(new AsyncCallback(Receive), null);
            Process(received);

        }

        //process recieved message
        public void Process(byte[] receivedBytes)
        {
            _readWrite.DecodeMessage(receivedBytes);
            Enqueue(IncomingQueue, _readWrite.targetMessage);
        }

        /*public bool Send(Message msg, IPEndPoint targetEndPoint)
        {
            msg.fromAddress = targetEndPoint;
            return Send(msg);
        }*/

        public void Send()
        {
            while(keepSending)
            {
                Message _msg = Dequeue(OutgoingQueue);
                if(_msg != null )
                {
                    Log.Debug("Entering Send");

                    bool result = false;

                   /* if (_msg.fromAddress != null) //adjust this for end point
                    {
                        try
                        {
                            Log.Debug($"Send {_msg} to {_msg.fromAddress}");
                            byte[] buffer = _readWrite.EncodeMessage(_msg);
                            Log.Debug($"Bytes sent: {FormatBytesForDisplay(buffer)}");
                            int count = _udpClient.Send(buffer, buffer.Length, _msg.fromAddress); //??
                            result = (count == buffer.Length);
                            Log.Info($"Sent {_msg.messageBody} of type '{_msg.thisMessageType}' to {_msg.fromAddress.Address}, result={result}");
                        }
                        catch (Exception err)
                        {
                            Log.Error("Unexpected exception while sending datagram - ", err);
                        }
                    }*/

                    Log.Debug("Leaving Send, result = " + result);
                } else
                {
                    Thread.Sleep(10);
                }
  
            }
   
        }

        public void Close()
        {
            _udpClient?.Close();
        }

        #endregion

        private string FormatBytesForDisplay(byte[] bytes)
        {
            return bytes.Aggregate(string.Empty, (current, b) => current + (b.ToString("X") + " "));
        }
    }
}
