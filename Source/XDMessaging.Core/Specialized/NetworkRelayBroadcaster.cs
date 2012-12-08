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
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Entities;
using XDMessaging.Messages;

namespace XDMessaging.Specialized
{
    /// <summary>
    ///   The implementation used to listen for and relay network messages for all
    ///   instances of IXDListener.
    /// </summary>
    internal sealed class NetworkRelayBroadcaster : IXDBroadcaster
    {
        public const string RelayChannel = "XDMessaging.Relay.Channel";
        private readonly ISerializer serializer;
        private readonly XDTransportMode originalTransportMode;
        private readonly IXDBroadcaster networkBroadcaster;

        public NetworkRelayBroadcaster(ISerializer serializer, XDTransportMode originalTransportMode, IXDBroadcaster networkBroadcaster)
        {
            Validate.That(serializer).IsNotNull();
            Validate.That(networkBroadcaster).IsNotNull();

            this.serializer = serializer;
            this.originalTransportMode = originalTransportMode;
            this.networkBroadcaster = networkBroadcaster;
        }

        public void SendToChannel(string channel, string message)
        {
            var networkMessage = new NetworkRelayMessage(Environment.MachineName, originalTransportMode, channel, message);
            networkBroadcaster.SendToChannel(RelayChannel, networkMessage);
        }

        public void SendToChannel(string channel, object message)
        {
            var networkMessage = new NetworkRelayMessage(Environment.MachineName, originalTransportMode, channel, serializer.Serialize(message));
            networkBroadcaster.SendToChannel(string.Concat(originalTransportMode, RelayChannel), networkMessage);
        }
    }
}