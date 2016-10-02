using System;
using System.Threading.Tasks;
using Conditions;
using XDMessaging.Messages;
using XDMessaging.Serialization;

namespace XDMessaging.Specialized
{
    internal sealed class NetworkRelayBroadcaster : IXDBroadcaster
    {
        private const string RelayChannel = "XDMessaging_RelayChannel";
        private readonly IXDBroadcaster networkBroadcaster;
        private readonly string networkChannel;
        private readonly XDTransportMode originalTransportMode;
        private readonly ISerializer serializer;

        public NetworkRelayBroadcaster(ISerializer serializer, XDTransportMode originalTransportMode,
            IXDBroadcaster networkBroadcaster)
        {
            serializer.Requires("serializer").IsNotNull();
            networkBroadcaster.Requires("networkBroadcaster").IsNotNull();

            this.serializer = serializer;
            this.originalTransportMode = originalTransportMode;
            this.networkBroadcaster = networkBroadcaster;
            networkChannel = GetNetworkListenerName(originalTransportMode);
        }

        public void SendToChannel(string channel, string message)
        {
            Task.Factory.StartNew(() =>
            {
                var networkMessage = new NetworkRelayMessage(Environment.MachineName,
                    originalTransportMode, channel,
                    message);

                networkBroadcaster.SendToChannel(networkChannel, networkMessage);
                // ReSharper disable once UnusedVariable
            }).ContinueWith(t => { var e = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void SendToChannel(string channel, object message)
        {
            Task.Factory.StartNew(() =>
            {
                var networkMessage = new NetworkRelayMessage(Environment.MachineName,
                    originalTransportMode, channel,
                    serializer.Serialize(message));

                networkBroadcaster.SendToChannel(networkChannel, networkMessage);
                // ReSharper disable once UnusedVariable
            }).ContinueWith(t => { var e = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public bool IsAlive => networkBroadcaster.IsAlive;

        internal static string GetNetworkListenerName(XDTransportMode transportMode)
        {
            return string.Concat(transportMode, "_", RelayChannel);
        }
    }
}