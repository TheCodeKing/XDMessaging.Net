/*=============================================================================
*
*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expresed or implied.
*
*-----------------------------------------------------------------------------
*	History:
*		11/02/2007	Michael Carlisle				Version 1.0
*       08/09/2007  Michael Carlisle                Version 1.1
*       12/12/2009  Michael Carlisle                Version 2.0
 *                  Added XDIOStream implementation which can be used from Windows Services.
*=============================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace TheCodeKing.Net.Messaging.Concrete.IOStream
{
    /// <summary>
    /// A concrete implementation of IXDListener and IXDBroadcast which can be used to send and recieve messages across
    /// appDomain and process boundaries using file IO streams to a shared directory. Applications may leverage this 
    /// instance to register listeners on pseudo 'channels', and receive messages sent from all processes using this
    /// IXDBroadcast implementation within the same machine.
    /// </summary>
    internal class XDIOStream : IXDBroadcast, IXDListener
    {
        // Flag as to whether dispose has been called
        private bool disposed = false;
        /// <summary>
        /// Unique mutex key to synchronize the clean up tasks across processes.
        /// </summary>
        private const string MutexCleanUpKey = "TheCodeKing.Net.XDServices.IOStream.Cleaner";
        /// <summary>
        /// Get a list of charactors that must be stripped from a channel name folder.
        /// </summary>
        private static readonly char[] invalidChannelChars = Path.GetInvalidFileNameChars();
        /// <summary>
        /// The timeout period after which messages are deleted. 
        /// </summary>
        private const int fileTimeoutMilliseconds = 5000;
        /// <summary>
        /// The temporary folder where messages will be stored.
        /// </summary>
        private readonly string tempFolder;
        /// <summary>
        /// A list of FileSystemWatcher instances used for each registered channel.
        /// </summary>
        private Dictionary<string, FileSystemWatcher> watcherList;
        /// <summary>
        /// A lock object used to ensure changes to watcherList are thread-safe.
        /// </summary>
        private object lockObj = new object();

        /// <summary>
        /// The constructor used to create a cnew instance of XDIOStream. Instances are created using
        /// factory methods on the XDListener or XDBroadcast classes.
        /// </summary>
        internal XDIOStream()
        {
            this.tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "XDMessaging");
            this.watcherList = new Dictionary<string, FileSystemWatcher>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// The implementation of IXDBroadcast, used to broadcast a new message to other processes. This creates a unique
        /// file on the filesystem. The temporary files are cleaned up after a pre-defined timeout. 
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="message"></param>
        public void SendToChannel(string channelName, string message)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(channelName, "The channel name must be defined");
            }
            if (message == null)
            {
                throw new ArgumentNullException(message, "The messsage packet cannot be null");
            }
            // create temporary name
            string fileName = Guid.NewGuid().ToString();
            string folder = GetChannelDirectory(channelName);
            string filePath = Path.Combine(folder, string.Concat(fileName, ".msg"));
            // write the message to the temp file, which will trigger listeners in other processes
            using (StreamWriter writer = File.CreateText(filePath))
            {
                // write out the channel name and message, this allows for invalid
                // characters in the channel name.
                writer.Write(string.Concat(channelName, ":", message));
                writer.Flush();
            }
            // return as fast as we can, leaving a clean up task
            ThreadPool.QueueUserWorkItem(CleanUp, new FileInfo(filePath).Directory);
        }
        /// <summary>
        /// This method is called within a seperate thread and deletes messages that are older than
        /// the pre-defined expiry time.
        /// </summary>
        /// <param name="state"></param>
        private void CleanUp(object state)
        {
            DirectoryInfo directory = state as DirectoryInfo;
            // use a mutex to ensure only one listener system wide is running
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, MutexCleanUpKey, out createdNew))
            {
                if (createdNew)
                {
                    try
                    {
                        // wait for the specified timeout before attempting to clean directory
                        Thread.Sleep(fileTimeoutMilliseconds);
                        // check directory not deleted, don't use cached version (directory.Exists)
                        if (Directory.Exists(directory.FullName))
                        {
                            foreach (FileInfo file in directory.GetFiles("*.msg"))
                            {
                                // attempt to clean up all expired messages in the channel directory
                                if (file.CreationTimeUtc <= DateTime.UtcNow.AddMilliseconds(-fileTimeoutMilliseconds))
                                {
                                    if (File.Exists(file.FullName))
                                    {
                                        try
                                        {
                                            file.Delete();
                                        }
                                        catch (IOException) { } // the file could have been deleted by another broadcaster, retry later.
                                        catch (UnauthorizedAccessException) { } // if the file is still in use retry again later.
                                    }
                                }
                            }
                        }
                    }
                    catch (IOException) { } // the file could have been deleted by another broadcaster, retry later.
                    catch (UnauthorizedAccessException) { } // if the file is still in use retry again later.
                }
            }
            if (createdNew)
            {
                // after mutex release add an additional thread, in case we're the last out to finalize cleanup
                ThreadPool.QueueUserWorkItem(CleanUp, directory);
            }
        }

        /// <summary>
        /// The MessageReceived event used to broadcast the message to attached instances within the current appDomain.
        /// </summary>
        public event XDListener.XDMessageHandler MessageReceived;

        /// <summary>
        /// Sets up a new FileSystemWatcher so that messages can be received on a particular 'channel'.
        /// </summary>
        /// <param name="channelName"></param>
        public void RegisterChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(channelName, "The channel name cannot be null or empty.");
            }
            if (disposed)
            {
                throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
            }
            FileSystemWatcher watcher = EnsureWatcher(channelName);
            watcher.EnableRaisingEvents = true;
        }
        /// <summary>
        /// Disables any FileSystemWatcher for a particular channel so that messages are no longer received.
        /// </summary>
        /// <param name="channelName"></param>
        public void UnRegisterChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(channelName, "The channel name cannot be null or empty.");
            }
            if (disposed)
            {
                throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
            }
            FileSystemWatcher watcher = EnsureWatcher(channelName);
            watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Provides a thread safe method to lookup/create a instance of FileSystemWatcher for a particular channel.
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        private FileSystemWatcher EnsureWatcher(string channelName)
        {
            FileSystemWatcher watcher = null;
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
                        string folder = GetChannelDirectory(channelName);
                        watcher = new FileSystemWatcher(folder, "*.msg");
                        watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite;
                        watcher.Changed += new FileSystemEventHandler(OnMessageReceived);
                        watcherList.Add(channelName, watcher);
                    }
                }
            }
            return watcher;
        }

        /// <summary>
        /// A helper method used to determine the temporary directory location used for
        /// a particular channel. The directory is created if it does not exist.
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        private string GetChannelDirectory(string channelName)
        {
            string folder = null;
            try
            {
                string channelKey = GetChannelKey(channelName);
                folder = Path.Combine(tempFolder, channelKey);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                return folder;
            }
            catch (PathTooLongException e)
            {
                throw new ArgumentException(string.Format("Unable to bind to channel as the name '{0}' is too long." +
                    " Try a shorter channel name.", channelName), e);
            }
            catch (UnauthorizedAccessException ue)
            {
                throw new UnauthorizedAccessException(string.Format("Unable to bind to channel '{0}' as access is denied." +
                    " Ensure the process has read/write access to the directory '{1}'.", channelName, folder), ue);
            }
            catch (IOException ie)
            {
                throw new IOException(string.Format("There was an unexpected IO error binding to channel '{0}'." +
                    " Ensure the process is unable to read/write to directory '{1}'.", channelName, folder), ie);
            }
        }

        /// <summary>
        /// The FileSystemWatcher event that is triggered when a new file is created in the channel temporary
        /// directory. This dispatches the MessageReceived event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender, FileSystemEventArgs e)
        {
            // if a new file is added to the channel directory
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                try 
                {
                    // check if file exists
                    if (File.Exists(e.FullPath))
                    {
                        string rawmessage = null;
                        // try to load the file in shared access mode
                        using (FileStream stream = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                rawmessage = reader.ReadToEnd();
                            }
                        }
                        // if the message contains valid data
                        if (!string.IsNullOrEmpty(rawmessage) && rawmessage.Contains(":"))
                        {
                            // extract the channel name and message data
                            string[] parts = rawmessage.Split(new[]{':'}, 2);
                            string message = parts[1];
                            string channel = parts[0];
                            if (MessageReceived != null)
                            {
                                using (DataGram dataGram = new DataGram(channel, message))
                                {
                                    // dispatch the message received event
                                     MessageReceived(this, new XDMessageEventArgs(dataGram));
                                }
                            }
                        }
                    }
                }
                catch (FileNotFoundException) 
                {
                    // if for any reason the file was deleted before the message could be read from the file,
                    // then can safely ignore this message
                }
                catch (UnauthorizedAccessException ue)
                {
                    throw new UnauthorizedAccessException(string.Format("Unable to bind to channel as access is denied." +
                        " Ensure the process has read/write access to the directory '{0}'.", e.FullPath), ue);
                }
                catch (IOException ie)
                {
                    throw new IOException(string.Format("There was an unexpected IO error binding to a channel." +
                        " Ensure the process is unable to read/write to directory '{0}'.", e.FullPath), ie);
                }
            }
        }
        /// <summary>
        /// Gets a channel key string associated with the channel name. This is used as the 
        /// directory name in the temporary directory, and we therefore strip out any invalid characters.
        /// </summary>
        /// <param name="channelName">The channel name for which a channel key is required.</param>
        /// <returns>The string channel key.</returns>
        internal static string GetChannelKey(string channelName)
        {
            foreach (char c in invalidChannelChars)
            {
                if (channelName.Contains(c.ToString()))
                {
                    channelName = channelName.Replace(c, '_');
                }
            }
            return channelName;
        }

        /// <summary>
        /// Deconstructor, cleans unmanaged resources only
        /// </summary>
        ~XDIOStream()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose implementation which ensures all FileSystemWatchers
        /// are shut down and handlers detatched.
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
                    if (watcherList != null)
                    {
                        // shut down watchers
                        foreach (FileSystemWatcher watcher in watcherList.Values)
                        {
                            watcher.EnableRaisingEvents = false;
                            watcher.Changed -= new FileSystemEventHandler(OnMessageReceived);
                            watcher.Dispose();
                        }
                        watcherList.Clear();
                        watcherList = null;
                    }
                }
            }
        }
    }
}
