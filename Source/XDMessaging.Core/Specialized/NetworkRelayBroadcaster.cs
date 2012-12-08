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
using System.Threading.Tasks;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Messages;

namespace XDMessaging.Specialized
{
    /// <summary>
    /// 	The implementation used to listen for and relay network messages for all
    /// 	instances of IXDListener.
    /// </summary>
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
            Validate.That(serializer).IsNotNull();
            Validate.That(networkBroadcaster).IsNotNull();

            this.serializer = serializer;
            this.originalTransportMode = originalTransportMode;
            this.networkBroadcaster = networkBroadcaster;
            networkChannel = GetNetworkListenerName(originalTransportMode);
        }

        #region IXDBroadcaster Members

        public void SendToChannel(string channel, string message)
        {
            Task.Factory.StartNew(() =>
                                      {
                                          var networkMessage = new NetworkRelayMessage(Environment.MachineName,
                                                                                       originalTransportMode, channel,
                                                                                       message);
                                          networkBroadcaster.SendToChannel(networkChannel, networkMessage);
                                      }).ContinueWith(t =>
                                                          {
                                                              var e = t.Exception;
                                                          }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void SendToChannel(string channel, object message)
        {
            Task.Factory.StartNew(() =>
                                      {
                                          var networkMessage = new NetworkRelayMessage(Environment.MachineName,
                                                                                       originalTransportMode, channel,
                                                                                       serializer.Serialize(message));
                                          networkBroadcaster.SendToChannel(networkChannel, networkMessage);
                                      }).ContinueWith(t =>
                                                          {
                                                              var e = t.Exception;
                                                          }, TaskContinuationOptions.OnlyOnFaulted);
            ;
        }

        #endregion

        internal static string GetNetworkListenerName(XDTransportMode transportMode)
        {
            return string.Concat(transportMode, "_", RelayChannel);
        }
    }
}