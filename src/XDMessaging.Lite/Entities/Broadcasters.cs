using System;
using Conditions;
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
                    return new XDIOStreamBroadcaster(serializer);
                default:
                    throw new ArgumentOutOfRangeException(nameof(transportMode), transportMode, null);
            }
        }
    }
}