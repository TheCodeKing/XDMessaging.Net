/*=============================================================================
*
*	(C) Copyright 2013, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
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
using System.Configuration;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Specialized;

namespace XDMessaging.Entities
{
    public sealed class Broadcasters : XDRegisterBase
    {
        private readonly XDMessagingClient messagingClient;

        #region Constructors and Destructors

        public Broadcasters(XDMessagingClient messagingClient, IocContainer container)
            : base(container)
        {
            Validate.That(messagingClient).IsNotNull();

            this.messagingClient = messagingClient;
        }

        #endregion

        #region Public Methods

        public IXDBroadcaster GetBroadcasterForMode(XDTransportMode transportMode)
        {
            return GetBroadcasterForMode(transportMode, false);
        }

        public IXDBroadcaster GetBroadcasterForMode(XDTransportMode transportMode, bool propagateNetwork)
        {
            var broadcaster = Container.Resolve<IXDBroadcaster>(Convert.ToString(transportMode));
            if (broadcaster == null)
            {
                throw new NotSupportedException(
                    string.Format(
                        "No concrete IXDBroadcast for mode {0} could be loaded. Install the {0} assembly in the program directory.",
                        transportMode));
            }
            if (propagateNetwork && transportMode != XDTransportMode.RemoteNetwork)
            {
                var remoteBroadcaster = messagingClient.Broadcasters.GetBroadcasterForMode(XDTransportMode.RemoteNetwork);              
                if (!remoteBroadcaster.IsAlive)
                {
                    throw new ConfigurationErrorsException("The RemoteNetwork Broadcaster is not configured. Check the configuration settings.");
                }
                var relayBroadcaster = new NetworkRelayBroadcaster(Container.Resolve<ISerializer>(), transportMode,
                                                                   remoteBroadcaster);
                broadcaster = messagingClient.Broadcasters.GetMulticastBroadcaster(broadcaster, relayBroadcaster);
            }
            return broadcaster;
        }

        #endregion
    }
}