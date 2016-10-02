using System;
using Conditions;
using XDMessaging.Entities.Amazon;
using XDMessaging.IdentityProviders;
using XDMessaging.Serialization;
using XDMessaging.Specialized;
using XDMessaging.Transport.Amazon;
using XDMessaging.Transport.Amazon.Facades;
using XDMessaging.Transport.Amazon.Repositories;
using XDMessaging.Transport.IOStream;
using XDMessaging.Transport.WindowsMessaging;

namespace XDMessaging.Entities
{
    public sealed class Listeners
    {
        // ReSharper disable once InconsistentNaming
        public delegate void XDMessageHandler(object sender, XDMessageEventArgs e);

        private readonly XDMessagingClient client;

        private readonly ISerializer serializer;

        internal Listeners(XDMessagingClient client, ISerializer serializer)
        {
            client.Requires("client").IsNotNull();
            serializer.Requires("serializer").IsNotNull();

            this.client = client;
            this.serializer = serializer;
        }

        public IXDListener GetListenerForMode(XDTransportMode transportMode)
        {
            var listener = GetListenerForModeInternal(transportMode);
            if (transportMode == XDTransportMode.RemoteNetwork)
            {
                return listener;
            }

            var networkListener = CreateNetworkListener(new MachineNameIdentityProvider());
            if (networkListener == null || !networkListener.IsAlive)
            {
                return listener;
            }

            var networkBroadcaster = client.Broadcasters.GetBroadcasterForMode(transportMode);
            listener = new NetworkRelayListener(networkBroadcaster, listener, networkListener, transportMode);

            return listener;
        }

        private IXDListener CreateNetworkListener(IIdentityProvider provider)
        {
            var settings = AmazonAccountSettings.GetInstance();
            var amazonSnsFacade = new AmazonSnsFacade(settings);
            var amazonSqsFacade = new AmazonSqsFacade(settings);
            var queuePoller = new QueuePoller(amazonSqsFacade);

            var resourceCounter = new ResourceCounter();
            var respository = new TopicRepository(AmazonAccountSettings.GetInstance(), amazonSnsFacade);
            var subscriberRepository = new SubscriberRepository(settings, amazonSqsFacade);
            var subscriptionService = new SubscriptionService(
                resourceCounter,
                amazonSnsFacade,
                amazonSqsFacade,
                subscriberRepository,
                queuePoller);

            return new XDAmazonListener(provider, serializer, respository, subscriberRepository, subscriptionService);
        }

        private IXDListener GetListenerForModeInternal(XDTransportMode transportMode)
        {
            switch (transportMode)
            {
                case XDTransportMode.HighPerformanceUI:
                    return new XDWinMsgListener(serializer);
                case XDTransportMode.Compatibility:
                    return new XDIOStreamListener(serializer);
                case XDTransportMode.RemoteNetwork:
                    return CreateNetworkListener(new UniqueIdentityProvider());
                default:
                    throw new ArgumentOutOfRangeException(nameof(transportMode), transportMode, null);
            }
        }
    }
}