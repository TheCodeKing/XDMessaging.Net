using System;
using Conditions;
using XDMessaging.Serialization;
using XDMessaging.Transport.IOStream;
using XDMessaging.Transport.WindowsMessaging;

namespace XDMessaging.Entities
{
    public sealed class Listeners
    {
        // ReSharper disable once InconsistentNaming
        public delegate void XDMessageHandler(object sender, XDMessageEventArgs e);

        private readonly ISerializer serializer;

        internal Listeners(XDMessagingClient client, ISerializer serializer)
        {
            client.Requires("client").IsNotNull();
            serializer.Requires("serializer").IsNotNull();

            this.serializer = serializer;
        }

        // ReSharper disable once InconsistentNaming
        public IXDListener GetIOStreamListener()
        {
            return GetListenerForMode(XDTransportMode.Compatibility);
        }

        public IXDListener GetListenerForMode(XDTransportMode transportMode)
        {
            switch (transportMode)
            {
                case XDTransportMode.HighPerformanceUI:
                    return new XDWinMsgListener(serializer);
                case XDTransportMode.Compatibility:
                    return new XDIOStreamListener(serializer);
                default:
                    throw new ArgumentOutOfRangeException(nameof(transportMode), transportMode, null);
            }
        }

        public IXDListener GetWindowsMessagingListener()
        {
            return GetListenerForMode(XDTransportMode.HighPerformanceUI);
        }
    }
}