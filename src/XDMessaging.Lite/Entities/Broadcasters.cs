using System;
using System.Configuration;
using Conditions;
using XDMessaging.Config;
using XDMessaging.Serialization;
using XDMessaging.Transport.IOStream;
using XDMessaging.Transport.WindowsMessaging;

namespace XDMessaging.Entities
{
    public sealed class Broadcasters
    {
        private readonly ISerializer serializer;

        internal Broadcasters(XDMessagingClient client, ISerializer serializer)
        {
            serializer.Requires("serializer").IsNotNull();

            this.serializer = serializer;
        }

        public IXDBroadcaster GetBroadcasterForMode(XDTransportMode transportMode)
        {
            switch (transportMode)
            {
                case XDTransportMode.HighPerformanceUI:
                    return new XDWinMsgBroadcaster(serializer);
                case XDTransportMode.Compatibility:
                    return new XDIOStreamBroadcaster(serializer, Settings.IoStreamMessageTimeoutInMilliseconds);
                default:
                    throw new ArgumentOutOfRangeException(nameof(transportMode), transportMode, null);
            }
        }
    }
}