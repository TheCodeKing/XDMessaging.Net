/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
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
using XDMessaging.Core.Interfaces;

namespace XDMessaging.Core.Concrete.MailSlot
{
    /// <summary>
    ///   Implementation of IXDListener. This uses a Mutex to synchronize access
    ///   to the MailSlot for a particular channel, such that only one listener will
    ///   pickup messages on a single machine per channel.
    /// </summary>
    internal sealed class XDMailSlotListener : IXDListener
    {
        #region Constants and Fields

        private readonly object lockObj = new object();
        private readonly object lockMsgObj = new object();

        /// <summary>
        /// A dictionary of DataGrams to be reassembled.
        /// </summary>
        private readonly Dictionary<Guid, MailSlotDataGram> messageParts;

        /// <summary>
        ///   A list of FileSystemWatcher instances used for each registered channel.
        /// </summary>
        private readonly Dictionary<string, MailSlotWatcher> watcherList;

        /// <summary>
        ///   Indicates whether the object has been disposed.
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   The default constructor.
        /// </summary>
        internal XDMailSlotListener()
        {
            watcherList = new Dictionary<string, MailSlotWatcher>(StringComparer.InvariantCultureIgnoreCase);
            messageParts = new Dictionary<Guid, MailSlotDataGram>();
        }

        /// <summary>
        ///   Deconstructor, cleans unmanaged resources only
        /// </summary>
        ~XDMailSlotListener()
        {
            Dispose(false);
        }

        #endregion

        #region Events

        /// <summary>
        ///   The delegate used to dispatch the MessageReceived event.
        /// </summary>
        public event XDListener.XDMessageHandler MessageReceived;

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        ///   Dispose implementation which ensures all resources are destroyed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IXDListener

        /// <summary>
        ///   Create a new listener thread which will try and obtain the mutex. If it can't
        ///   because another process is already polling this channel then it will wait until 
        ///   it can gain an exclusive lock.
        /// </summary>
        /// <param name = "channelName"></param>
        public void RegisterChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentException("The channel name cannot be null or empty.", "channelName");
            }
            if (disposed)
            {
                throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
            }
            MailSlotWatcher watcher = EnsureWatcher(channelName);
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        ///   Unregisters the current instance from the given channel. No more messages will be 
        ///   processed, and another process will be allowed to obtain the listener lock.
        /// </summary>
        /// <param name = "channelName"></param>
        public void UnRegisterChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                 throw new ArgumentException("The channel name cannot be null or empty.", "channelName");
            }
            MailSlotWatcher watcher = EnsureWatcher(channelName);
            watcher.EnableRaisingEvents = false;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Add fragment to master copy based on unique id.
        /// </summary>
        /// <param name="dataGram"></param>
        private void DeFragmentMessage(MailSlotDataGram dataGram)
        {
            MailSlotDataGram item;
            if (!messageParts.TryGetValue(dataGram.Id, out item))
            {
                lock (lockMsgObj)
                {
                    if (!messageParts.TryGetValue(dataGram.Id, out item))
                    {
                        messageParts.Add(dataGram.Id, dataGram);
                        item = dataGram;
                    }
                }
            }

            lock (item)
            {
                // add this fragment
                if (dataGram.Index != item.Index)
                {
                    item.AddFragment(dataGram.Index, dataGram.Message);
                }
                if (item.IsComplete)
                {
                    messageParts.Remove(item.Id);
                    OnMessageReceived(item);
                }
            }
        }

        /// <summary>
        ///   Dispose implementation, which ensures the native window is destroyed
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
                        foreach (XDListener.XDMessageHandler msg in del)
                        {
                            MessageReceived -= msg;
                        }

                        if (watcherList != null)
                        {
                            // shut down watchers
                            foreach (var watcher in watcherList.Values)
                            {
                                watcher.EnableRaisingEvents = false;
                                watcher.DataReceived -= OnMessageReceived;
                                watcher.Dispose();
                            }
                            watcherList.Clear();
                        }
                    }
                }
            }
        }

        private MailSlotWatcher EnsureWatcher(string channelName)
        {
            MailSlotWatcher watcher;
            // try to get a reference to the watcher used for the current watcher
            if (!watcherList.TryGetValue(channelName, out watcher))
            {
                // if no watcher then lock the list
                lock (lockObj)
                {
                    // whilst locked double check if the item has been added since the lock was applied
                    if (!watcherList.TryGetValue(channelName, out watcher))
                    {
                        // create a new watcher for the given channel, by default this is not enabled.
                        string location = XDMailSlotBroadcast.GetChannelPath(".", channelName);
                        watcher = new MailSlotWatcher(location);
                        watcher.DataReceived += OnMessageReceived;
                        watcherList.Add(channelName, watcher);
                    }
                }
            }
            return watcher;
        }

        private void OnMessageReceived(object sender, MailSlotDataReceivedEventArgs args)
        {
            // process the message async, to return as quickly as possible
            Action<string> action = ProcessMessage;
            action.BeginInvoke(args.Data, action.EndInvoke, null);
        }

        /// <summary>
        ///   This method processes the message and triggers the MessageReceived event.
        /// </summary>
        /// <param name = "dataGram"></param>
        private void OnMessageReceived(DataGram dataGram)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, new XDMessageEventArgs(dataGram));
            }
        }

        private void ProcessMessage(string message)
        {
            MailSlotDataGram dataGram = MailSlotDataGram.ExpandFromRaw(message);

            if (dataGram.IsValid)
            {
                DeFragmentMessage(dataGram);
            }
        }

        #endregion
    }
}