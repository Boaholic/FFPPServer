using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FFPPServer
{
    /// <summary>
    /// This class is responsible for executing, undoing, and redoing commands.  It performs this actions on a separate thread.
    /// 
    /// Note: this class does not depend on the Drawing Components, any concrete command classes or the GUI.  Therefore, it is
    /// highly reusable and easy to test.
    /// 
    /// </summary>
    public class Invoker
    {
        private Thread _worker;
        private bool _keepGoing;

        private readonly ConcurrentQueue<Conversation> _todoQueue = new ConcurrentQueue<Conversation>();
        private readonly AutoResetEvent _enqueueOccurred = new AutoResetEvent(false);

        private readonly Stack<Conversation> _undoStack = new Stack<Conversation>();
        private readonly Stack<Conversation> _redoStack = new Stack<Conversation>();

        public void Start()
        {
            _keepGoing = true;
            _worker = new Thread(Run);
            _worker.Start();
        }

        public void Stop()
        {
            _keepGoing = false;
        }

        public void EnqueueCommandForExecution(Conversation cmd)
        {
            if (cmd != null)
            {
                _todoQueue.Enqueue(cmd);
                _enqueueOccurred.Set();
            }
        }

      

        private void Run()
        {
            while (_keepGoing)
            {
                Conversation cmd;
                if (_todoQueue.TryDequeue(out cmd))
                {

                    cmd.Execute();
                }
                else
                    _enqueueOccurred.WaitOne(100);
            }
        }


    }
}
