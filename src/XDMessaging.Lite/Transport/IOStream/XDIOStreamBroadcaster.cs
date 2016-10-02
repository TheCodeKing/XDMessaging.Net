using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Conditions;
using XDMessaging.Messages;
using XDMessaging.Serialization;

namespace XDMessaging.Transport.IOStream
{
    // ReSharper disable once InconsistentNaming
    public sealed class XDIOStreamBroadcaster : IXDBroadcaster
    {
        private const int FileTimeoutMilliseconds = 5000;

        private const string MutexCleanUpKey = @"Global\XDIOStreamBroadcastv4.Cleanup";

        private static readonly char[] InvalidChannelChars = Path.GetInvalidFileNameChars();

        private static readonly string TemporaryFolder;

        private readonly ISerializer serializer;

        static XDIOStreamBroadcaster()
        {
            TemporaryFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "XDMessagingv4");
        }

        internal XDIOStreamBroadcaster(ISerializer serializer)
        {
            serializer.Requires("serializer").IsNotNull();

            this.serializer = serializer;
        }

        public void SendToChannel(string channelName, object message)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();
            message.Requires("message").IsNotNull();

            SendToChannel(channelName, message.GetType().AssemblyQualifiedName, serializer.Serialize(message));
        }

        public void SendToChannel(string channelName, string message)
        {
            SendToChannel(channelName, typeof (string).AssemblyQualifiedName, message);
        }

        public bool IsAlive => true;

        internal static string GetChannelDirectory(string channelName)
        {
            string folder = null;
            try
            {
                var channelKey = GetChannelKey(channelName);
                folder = Path.Combine(TemporaryFolder, channelKey);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                return folder;
            }
            catch (PathTooLongException e)
            {
                throw new ArgumentException(
                    $"Unable to bind to channel as the name '{channelName}' is too long." +
                    " Try a shorter channel name.", e);
            }
            catch (UnauthorizedAccessException ue)
            {
                throw new UnauthorizedAccessException(
                    $"Unable to bind to channel '{channelName}' as access is denied." +
                    $" Ensure the process has read/write access to the directory '{folder}'.", ue);
            }
            catch (IOException ie)
            {
                throw new IOException(
                    $"There was an unexpected IO error binding to channel '{channelName}'." +
                    $" Ensure the process is unable to read/write to directory '{folder}'.", ie);
            }
        }

        internal static string GetChannelKey(string channelName)
        {
            foreach (var c in InvalidChannelChars)
            {
                if (channelName.Contains(c.ToString()))
                {
                    channelName = channelName.Replace(c, '_');
                }
            }
            return channelName;
        }

        private static void CleanUpMessages(object state)
        {
            var directory = (DirectoryInfo) state;

            bool createdNew;
            var mutexName = string.Concat(MutexCleanUpKey, ".", directory.Name);
            var accessControl = new MutexSecurity();
            var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            accessControl.SetAccessRule(new MutexAccessRule(sid, MutexRights.FullControl, AccessControlType.Allow));
            using (var mutex = new Mutex(true, mutexName, out createdNew, accessControl))
            {
                if (createdNew)
                {
                    try
                    {
                        Thread.Sleep(FileTimeoutMilliseconds);
                    }
                    catch (ThreadInterruptedException)
                    {
                    }
                    CleanUpMessages(directory);
                    mutex.ReleaseMutex();
                }
            }
            if (createdNew)
            {
                ThreadPool.QueueUserWorkItem(CleanUpMessages, directory);
            }
        }

        private static void CleanUpMessages(DirectoryInfo directory)
        {
            try
            {
                if (!Directory.Exists(directory.FullName))
                {
                    return;
                }

                foreach (var file in directory.GetFiles("*.msg"))
                {
                    if (file.CreationTimeUtc > DateTime.UtcNow.AddMilliseconds(-FileTimeoutMilliseconds)
                        || !File.Exists(file.FullName))
                    {
                        continue;
                    }

                    try
                    {
                        file.Delete();
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private void SendToChannel(string channelName, string dataType, string message)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();
            dataType.Requires("dataType").IsNotNullOrWhiteSpace();
            message.Requires("message").IsNotNullOrWhiteSpace();

            var fileName = Guid.NewGuid().ToString();
            var folder = GetChannelDirectory(channelName);
            var filePath = Path.Combine(folder, string.Concat(fileName, ".msg"));

            using (var writer = File.CreateText(filePath))
            {
                var dataGram = new DataGram(channelName, dataType, message);
                writer.Write(serializer.Serialize(dataGram));
                writer.Flush();
            }

            ThreadPool.QueueUserWorkItem(CleanUpMessages, new FileInfo(filePath).Directory);
        }
    }
}