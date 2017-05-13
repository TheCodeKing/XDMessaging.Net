using System;
using System.Configuration;
using Conditions;
using XDMessaging.Config;
using XDMessaging.Entities.Amazon;
using XDMessaging.Serialization;
using XDMessaging.Specialized;
using XDMessaging.Transport.Amazon;
using XDMessaging.Transport.Amazon.Facades;
using XDMessaging.Transport.Amazon.Repositories;
using XDMessaging.Transport.IOStream;
using XDMessaging.Transport.WindowsMessaging;

namespace XDMessaging.Entities
{
    public sealed class Broadcasters
    {
        private readonly XDMessagingClient client;
        private readonly ISerializer serializer;

        internal Broadcasters(XDMessagingClient client, ISerializer serializer)
        {
            client.Requires("client").IsNotNull();
            serializer.Requires("serializer").IsNotNull();

            this.client = client;
            this.serializer = serializer;
        }

        public IXDBroadcaster GetBroadcasterForMode(XDTransportMode transportMode)
        {
            return GetBroadcasterForMode(transportMode, false);
        }

        public IXDBroadcaster GetBroadcasterForMode(XDTransportMode transportMode, bool propagateNetwork)
        {
            var broadcaster = GetBroadcasterForModeInternal(transportMode);
            if (!propagateNetwork || transportMode == XDTransportMode.RemoteNetwork)
            {
                return broadcaster;
            }

            var remoteBroadcaster = CreateNetworkBroadcaster();
            if (!remoteBroadcaster.IsAlive)
            {
                throw new ConfigurationErrorsException(
                    "The RemoteNetwork Broadcaster is not configured. Check the configuration settings.");
            }

            var relayBroadcaster = new NetworkRelayBroadcaster(
                serializer,
                transportMode,
                remoteBroadcaster);

            return client.Broadcasters.GetMulticastBroadcaster(broadcaster, relayBroadcaster);
        }

        private IXDBroadcaster CreateNetworkBroadcaster()
        {
            var settings = AmazonAccountSettings.GetInstance();
            var amazonSnsFacade = new AmazonSnsFacade(settings);
            var respository = new TopicRepository(settings, amazonSnsFacade);
            var publishService = new PublisherService(amazonSnsFacade);

            return new XDAmazonBroadcaster(serializer, publishService, respository);
        }

        private IXDBroadcaster GetBroadcasterForModeInternal(XDTransportMode transportMode)
        {
            switch (transportMode)
            {
                case XDTransportMode.HighPerformanceUI:
                    return new XDWinMsgBroadcaster(serializer);
                case XDTransportMode.Compatibility:
                    return new XDIOStreamBroadcaster(serializer, Settings.IoStreamMessageTimeoutInMilliseconds);
                case XDTransportMode.RemoteNetwork:
                    return CreateNetworkBroadcaster();
                default:
                    throw new ArgumentOutOfRangeException(nameof(transportMode), transportMode, null);
            }
        }
    }
}