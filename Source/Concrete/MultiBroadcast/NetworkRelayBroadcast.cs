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
using System.IO;
using System.Threading;
using TheCodeKing.Net.Messaging.Concrete.MailSlot;
using TheCodeKing.Net.Messaging.Interfaces;

namespace TheCodeKing.Net.Messaging.Concrete.MultiBroadcast
{
    /// <summary>
    ///   This implementation is used to broadcast messages from other implementations across
    ///   the network using an internal MailSlot.
    /// </summary>
    internal sealed class NetworkRelayBroadcast : IXDBroadcast
    {
        #region Constants and Fields

        /// <summary>
        ///   The base channel name used for propagating messages.
        /// </summary>
        private const string networkPropagateChannelPrefix = "System.PropagateBroadcast.";

        private readonly XDTransportMode mode;

        /// <summary>
        ///   The MailSlot implementation used to broadcast messages across the local network.
        /// </summary>
        private readonly IXDBroadcast networkBroadcast;

        private readonly ISerializerHelper serializerHelper;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   The default constructor used to wrap a native broadcast implementation.
        /// </summary>
        /// <param name = "serializerHelper"></param>
        /// <param name = "networkBroadcast"></param>
        /// <param name = "mode"></param>
        internal NetworkRelayBroadcast(ISerializerHelper serializerHelper, IXDBroadcast networkBroadcast, XDTransportMode mode)
        {
            if (networkBroadcast == null)
            {
                throw new ArgumentNullException("networkBroadcast");
            }
            if (serializerHelper == null)
            {
                throw new ArgumentNullException("serializerHelper");
            }

            this.serializerHelper = serializerHelper;

            // the MailSlot broadcast implementation is used to send over the network
            this.networkBroadcast = networkBroadcast;
            this.mode = mode;
        }

        #endregion

        #region Public Methods

        public static string GetNetworkPropagationSlotForMode(XDTransportMode mode)
        {
            return string.Concat(networkPropagateChannelPrefix, mode.ToString());
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcast

        public void SendToChannel(string channelName, object message)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentException("The channel name must be defined", "channelName");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message", "The messsage packet cannot be null");
            }
            if (channelName.Contains(":"))
            {
                throw new ArgumentException("The channel name may not contain the ':' character.", "channelName");
            }

            SendToChannel(channelName, serializerHelper.Serialize(message));
        }

        /// <summary>
        ///   The IXDBroadcast implementation that additionally propagates messages
        ///   over the local network as well as the local machine.
        /// </summary>
        /// <param name = "channelName"></param>
        /// <param name = "message"></param>
        public void SendToChannel(string channelName, string message)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentException("The channel name must be defined", "channelName");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message", "The messsage packet cannot be null");
            }
            if (channelName.Contains(":"))
            {
                throw new ArgumentException("The channel name may not contain the ':' character.", "channelName");
            }
            // start the network propagation
            ThreadPool.QueueUserWorkItem(state => SafeNetworkPropagation(channelName, message));
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///   Attempts to propagate the message across the network using MailSlots. This may fail
        ///   under load in which case the message is dropped.
        /// </summary>
        /// <param name = "channelName"></param>
        /// <param name = "message"></param>
        private void SafeNetworkPropagation(string channelName, string message)
        {
            // if mailslot cannot be written to, handle these gracefully
            // dropping the message
            try
            {
                var dataGram = new NetworkRelayDataGram(XDMailSlotBroadcast.LocalScope, channelName, message);

                // broadcast system message over network using propagation channel
                var location = GetNetworkPropagationSlotForMode(mode);
                networkBroadcast.SendToChannel(location, dataGram.ToString());
            }
            catch (IOException)
            {
            }
        }

        #endregion
    }
}