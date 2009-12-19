/*=============================================================================
*
*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TheCodeKing.Net.Messaging.Concrete.WindowsMessaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    /// <summary>
    /// Implementation of IXDListener. This uses a Mutex to synchronize access
    /// to the MailSlot for a particualt channel, such that only one listener will
    /// ever poll for messages on a single machine.
    /// </summary>
    internal sealed class XDMailSlotListener : IXDListener
    {
        /// <summary>
        /// Indicates whether the object has been disposed.
        /// </summary>
        private bool disposed;
        /// <summary>
        /// Lock object used for synchronizing access to the activeThreads list.
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// The unique name of the Mutex used for locking access to the MailSlot for a named
        /// channel.
        /// </summary>
        private const string MutexNetworkDispatcher = "TheCodeKing.Net.XDServices.XDMailSlot.Listener";
        /// <summary>
        /// The base name of the MailSlot on the current machine.
        /// </summary>
        private static readonly string mailSlotIdentifier = string.Concat(@"\\.", XDMailSlotBroadcast.SlotLocation);
        /// <summary>
        /// A list of Thread instances used for reading the MailSlot.
        /// </summary>
        private Dictionary<string, Thread> activeThreads;
        /// <summary>
        /// The delegate used to dispatch the MessageReceived event.
        /// </summary>
        public event XDListener.XDMessageHandler MessageReceived;

        /// <summary>
        /// The default constructor.
        /// </summary>
        internal XDMailSlotListener()
        {
            this.activeThreads = new Dictionary<string, Thread>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// The worker thread entry point for polling the MailSlot. Threads will queue until a Mutex becomes
        /// available for a particular channel. 
        /// </summary>
        /// <param name="state"></param>
        private void MailSlotChecker(object state)
        {
            string channelName = (string)state;
            bool createdNew = true;
            // get a mutex name for the current channel
            string mutextKey = string.Concat(MutexNetworkDispatcher, ".", channelName);
            // Obtain the mutex, createdNew indicates whether this thread is the owner.
            using (Mutex mutex = new Mutex(true, mutextKey, out createdNew))
            {
                // if this thread does not own the mutx wait for it to become
                // available
                if (!createdNew)
                {
                    try
                    {
                        mutex.WaitOne();
                    }
                    catch (ThreadInterruptedException) { } // thread shutting down
                    catch (AbandonedMutexException) { } // mutex has been released by another process
                }
                // we are now the single thread responsible for checking the MailSlot for these channel
                // open up the MailSlot, read timeout does not work after first read so we have to poll
                IntPtr readHandle = IntPtr.Zero;

                try
                {
                    readHandle = Native.CreateMailslot(string.Concat(mailSlotIdentifier, channelName), 0, 0, IntPtr.Zero);

                    // If this thread is still registered, then go ahead the begin checking, else exit
                    while (activeThreads.ContainsKey(channelName))
                    {
                        byte[] buffer = new byte[512];
                        int numWaitingMessages = 0;
                        if ((int)readHandle > 0)
                        {
                            // check whether the MailSlot has unread messages
                            int timeout = 0;
                            int maxMesages = 0;
                            int nextSize = 0;
                            if (Native.GetMailslotInfo(readHandle, ref maxMesages, ref nextSize, ref numWaitingMessages, ref timeout))
                            {
                                // while it has unread messages, read them
                                while (numWaitingMessages > 0)
                                {
                                    string rawmessage = null;
                                    uint bytesRead = 0;
                                    // deserialize the data back into a string
                                    BinaryFormatter b = new BinaryFormatter();
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        do
                                        {
                                            // whilst there is still data to read, add it to the buffer
                                            if (Native.ReadFile(readHandle, buffer, (uint)buffer.Length, out bytesRead, IntPtr.Zero))
                                            {
                                                stream.Write(buffer, 0, (int)bytesRead);
                                            }
                                        }
                                        while (bytesRead > 0);

                                        stream.Flush();
                                        // reset the stream cursor back to the beginning
                                        stream.Seek(0, SeekOrigin.Begin);
                                        try
                                        {
                                            rawmessage = (string)b.Deserialize(stream);
                                        }
                                        catch (SerializationException) { } // if something goes wrong such as handle is closed,
                                        // we will not process this message
                                    }
                                    using (DataGram dataGram = DataGram.ExpandFromRaw(rawmessage))
                                    {
                                        if (dataGram.IsValid)
                                        {
                                            OnMessageReceived(dataGram);
                                        }
                                    }
                                    // move to next queued message
                                    numWaitingMessages--;
                                }
                                try
                                {
                                    Thread.Sleep(200);
                                }
                                catch (ThreadInterruptedException) { }
                                catch (ThreadAbortException) { }
                            }
                        }
                    }
                }
                finally
                {
                    if ((int)readHandle > 0)
                    {
                        // close the file handle
                        Native.CloseHandle(readHandle);
                    }
                }
            }
        }

        /// <summary>
        /// This method processes the message and triggers the MessageReceived event. 
        /// </summary>
        /// <param name="dataGram"></param>
        private void OnMessageReceived(DataGram dataGram)
        {
            if (MessageReceived != null)
            {
                // trigger this async
                MessageReceived.Invoke(this, new XDMessageEventArgs(dataGram));
            }
        }
        /// <summary>
        /// Create a new listener thread which will try and obtain the mutex. If it can't
        /// because another process is already polling this channel then it will wait until 
        /// it can gain an exclusive lock.
        /// </summary>
        /// <param name="channelName"></param>
        public void RegisterChannel(string channelName)
        {
            Thread channelThread;
            if (!activeThreads.TryGetValue(channelName, out channelThread))
            {
                // only lock if changing
                lock (lockObj)
                {
                    // double check has not been modified before lock
                    if (!activeThreads.TryGetValue(channelName, out channelThread))
                    {
                        channelThread = StartNewThread(channelName);
                        activeThreads[channelName] = channelThread;
                    }

                }
            }
        }
        /// <summary>
        /// Unregisters the current instance from the given channel. No more messages will be 
        /// processed, and another process will be allowed to obtain the listener lock.
        /// </summary>
        /// <param name="channelName"></param>
        public void UnRegisterChannel(string channelName)
        {
            Thread channelThread;
            if (activeThreads.TryGetValue(channelName, out channelThread))
            {
                // only lock if changing
                lock (lockObj)
                {
                    // double check has not been modified before lock
                    if (activeThreads.TryGetValue(channelName, out channelThread))
                    {
                        // removing the channel, shuts down the thread
                        activeThreads.Remove(channelName);
                        if (channelThread.IsAlive)
                        {
                            // interrupt incase of asleep thread
                            channelThread.Interrupt();
                        }
                        if (channelThread.IsAlive)
                        {
                            // attempt to join
                            if (!channelThread.Join(200))
                            {
                                // if no response within timeout, force abort
                                channelThread.Abort();
                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// Helper method starts up a new listener thread for a given channel.
        /// </summary>
        /// <param name="channelName">The channel name.</param>
        /// <returns></returns>
        private Thread StartNewThread(string channelName)
        {
            // create and start the thread at low priority
            Thread thread = new Thread(new ParameterizedThreadStart(MailSlotChecker));
            thread.Priority = ThreadPriority.Lowest;
            thread.IsBackground = true;
            thread.Start(channelName);
            return thread;
        }
        /// <summary>
        /// Deconstructor, cleans unmanaged resources only
        /// </summary>
        ~XDMailSlotListener()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose implementation which ensures all resources are destroyed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Dispose implementation, which ensures the native window is destroyed
        /// </summary>
        private void Dispose(bool disposeManaged)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposeManaged)
                {
                    if (MessageReceived != null)
                    {
                        // remove all handlers
                        Delegate[] del = MessageReceived.GetInvocationList();
                        foreach (TheCodeKing.Net.Messaging.XDListener.XDMessageHandler msg in del)
                        {
                            MessageReceived -= msg;
                        }
                    }
                    if (activeThreads != null)
                    {
                        // grab a reference to the current list of threads
                        var values = new List<Thread>(activeThreads.Values);
       
                        // removing the channels, will cause threads to terminate
                        activeThreads.Clear();
                        // shut down listener threads
                        foreach (Thread channelThread in values)
                        {
                            // ensure threads shut down 
                            if (channelThread.IsAlive)
                            {
                                // interrupt incase of asleep thread
                                channelThread.Interrupt();
                            }
                            // try to join thread
                            if (channelThread.IsAlive)
                            {
                                if (!channelThread.Join(200))
                                {
                                    // last resort abort thread
                                    channelThread.Abort();
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
