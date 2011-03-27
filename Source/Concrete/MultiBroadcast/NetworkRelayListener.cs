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

namespace TheCodeKing.Net.Messaging.Concrete.MultiBroadcast
{
    /// <summary>
    ///   The implementation used to listen for and relay network messages for all
    ///   instances of IXDListener.
    /// </summary>
    internal sealed class NetworkRelayListener : IXDListener
    {
        #region Constants and Fields

        /// <summary>
        ///   The factory instance used to create broadcast instances in order to re-send network messages natively.
        /// </summary>
        private readonly IXDBroadcast nativeBroadcast;

        /// <summary>
        ///   The instance of MailSlot used to receive network messages from other machines.
        /// </summary>
        private readonly IXDListener propagateListener;

        private readonly IXDListener nativeListener;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Default constructor.
        /// </summary>
        /// <param name = "nativeBroadcast"></param>
        /// <param name = "nativeListener"></param>
        /// <param name = "propagateListener"></param>
        /// <param name = "mode"></param>
        internal NetworkRelayListener(IXDBroadcast nativeBroadcast,
                                      IXDListener nativeListener,
                                      IXDListener propagateListener, XDTransportMode mode)
        {
            if (nativeBroadcast == null)
            {
                throw new ArgumentNullException("nativeBroadcast");
            }
            if (nativeListener == null)
            {
                throw new ArgumentNullException("nativeListener");
            }
            if (propagateListener == null)
            {
                throw new ArgumentNullException("propagateListener");
            }
            this.nativeBroadcast = nativeBroadcast;
            this.propagateListener = propagateListener;
            this.nativeListener = nativeListener;
            // listen on the network channel for this mode
            this.propagateListener.RegisterChannel(NetworkRelayBroadcast.GetNetworkPropagationSlotForMode(mode));
            this.propagateListener.MessageReceived += OnNetworkMessageReceived;
            this.nativeListener.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, XDMessageEventArgs e)
        {
            if (MessageReceived!=null)
            {
                MessageReceived(sender, e);
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        ///   Implementation of IDisposable used to clean up the listener instance.
        /// </summary>
        public void Dispose()
        {
            if (nativeListener != null)
            {
                nativeListener.Dispose();
            }
            if (propagateListener != null)
            {
                propagateListener.Dispose();
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///   Handles messages received from other machines on the network and dispatches them locally.
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void OnNetworkMessageReceived(object sender, XDMessageEventArgs e)
        {
            if (e.DataGram.IsValid)
            {
                NetworkRelayDataGram dataGram = e.DataGram;
                // don't relay if the message was broadcast on this machine
                if (dataGram.IsValid && dataGram.MachineName != Environment.MachineName)
                {
                    nativeBroadcast.SendToChannel(dataGram.Channel, dataGram.Message);
                }
            }
        }

        #endregion

        public event XDListener.XDMessageHandler MessageReceived;

        public void RegisterChannel(string channelName)
        {
            nativeListener.RegisterChannel(channelName);
        }

        public void UnRegisterChannel(string channelName)
        {
            nativeListener.UnRegisterChannel(channelName);
        }
    }
}