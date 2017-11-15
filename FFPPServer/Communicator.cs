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
        private static readonly ILog Log = LogManager.GetLogger(typeof(Communicator));

        private int _localPort;
        private static IPEndPoint _localEndPoint;
        private static UdpClient _udpClient;

        private static MessageQueue _incomingQueue = new MessageQueue();
        private static MessageQueue _outgoingQueue = new MessageQueue();
        private ReadWrite _readWrite = new ReadWrite();
        private DataProcessor _processor = new DataProcessor();


        private Thread _dataProcessor;
        private Thread _sender;
        
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Communicator()
        {
            Initialize();
        }

        /// <summary>
        /// Primary Constructor
        /// </summary>
        /// <param name="localPort">If non-zero, the communicator will attempt to use this port</param>
        public Communicator(int localPort)
        {
            _localPort = localPort;
            Initialize();
        }

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
            _processor._incomingMessages = _incomingQueue;
            _processor._outgoingMessages = _outgoingQueue;

            _dataProcessor = new Thread(new ThreadStart(_processor.Process));
            _dataProcessor.Start();

            _sender = new Thread(new ThreadStart(Send));
            _sender.Start();
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

        public void Listen(int timeout)
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
            Enqueue(_incomingQueue, _readWrite.targetMessage);
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
                Message _msg = Dequeue(_outgoingQueue);
                if(_msg != null )
                {
                    Log.Debug("Entering Send");

                    bool result = false;

                    if (_msg.fromAddress != null) //adjust this for end point
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
                    }

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
