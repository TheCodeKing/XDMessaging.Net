using System;
using System.Collections.Generic;
using System.IO;
using Conditions;
using log4net;
using XDMessaging.Entities;
using XDMessaging.Messages;
using XDMessaging.Serialization;

namespace XDMessaging.Transport.IOStream
{
    // ReSharper disable once InconsistentNaming
    public sealed class XDIOStreamListener : IXDListener
    {
        private readonly object disposeLock = new object();
        private readonly object lockObj = new object();

        private readonly ISerializer serializer;
        private bool disposed;

        private Dictionary<string, FileSystemWatcher> watcherList;

        private static ILog Log = LogManager.GetLogger(typeof(XDIOStreamListener));

        internal XDIOStreamListener(ISerializer serializer)
        {
            serializer.Requires().IsNotNull();

            this.serializer = serializer;
            watcherList = new Dictionary<string, FileSystemWatcher>(StringComparer.InvariantCultureIgnoreCase);
        }

        public event Listeners.XDMessageHandler MessageReceived;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RegisterChannel(string channelName)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();

            if (disposed)
            {
                return;
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    return;
                }

                var watcher = EnsureWatcher(channelName);
                watcher.EnableRaisingEvents = true;
            }
        }

        public void UnRegisterChannel(string channelName)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();

            if (disposed)
            {
                throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
                }

                var watcher = EnsureWatcher(channelName);
                watcher.EnableRaisingEvents = false;
            }
        }

        public bool IsAlive => true;

        private void Dispose(bool disposeManaged)
        {
            if (disposed)
            {
                return;
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                if (!disposeManaged)
                {
                    return;
                }

                if (MessageReceived != null)
                {
                    var del = MessageReceived.GetInvocationList();
                    foreach (var item in del)
                    {
                        var msg = (Listeners.XDMessageHandler) item;
                        MessageReceived -= msg;
                    }
                }
                if (watcherList == null)
                {
                    return;
                }

                foreach (var watcher in watcherList.Values)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Changed -= OnMessageReceived;
                    watcher.Dispose();
                }

                watcherList.Clear();
                watcherList = null;
            }
        }

        private FileSystemWatcher EnsureWatcher(string channelName)
        {
            FileSystemWatcher watcher;
            if (watcherList.TryGetValue(channelName, out watcher))
            {
                return watcher;
            }

            lock (lockObj)
            {
                if (watcherList.TryGetValue(channelName, out watcher))
                {
                    return watcher;
                }

                var folder = XDIOStreamBroadcaster.GetChannelDirectory(channelName);
                watcher = new FileSystemWatcher(folder, "*.msg")
                {
                    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite
                };

                watcher.Changed += OnMessageReceived;
                watcherList.Add(channelName, watcher);
            }

            return watcher;
        }

        private void OnMessageReceived(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            Action<string> action = ProcessMessage;
            action.BeginInvoke(e.FullPath, action.EndInvoke, null);
        }

        private void ProcessMessage(string fullPath)
        {
            try
            {
                if (!File.Exists(fullPath))
                {
                    return;
                }

                string rawmessage;
                using (var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        rawmessage = reader.ReadToEnd();
                    }
                }

                var dataGram = serializer.Deserialize<DataGram>(rawmessage);
                if (dataGram!=null && dataGram.IsValid)
                {
                    MessageReceived?.Invoke(this, new XDMessageEventArgs(dataGram));
                }
            }
            catch (FileNotFoundException)
            {
            }
            catch (UnauthorizedAccessException ue)
            {
                throw new UnauthorizedAccessException(
                    "Unable to bind to channel as access is denied." +
                    $" Ensure the process has read/write access to the directory '{fullPath}'.", ue);
            }
            catch (IOException ie)
            {
                Log.Error(new IOException(
                    "There was an unexpected IO error binding to a channel." +
                    $" Ensure the process is unable to read/write to directory '{fullPath}'.", ie));
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }

        ~XDIOStreamListener()
        {
            Dispose(false);
        }
    }
}