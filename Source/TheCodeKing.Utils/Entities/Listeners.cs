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
using TheCodeKing.Utils.IoC;
using XDMessaging.IdentityProviders;
using XDMessaging.Specialized;

namespace XDMessaging.Entities
{
    public sealed class Listeners : XDRegisterBase
    {
        #region Delegates

        /// <summary>
        /// 	The delegate used for handling cross AppDomain communications.
        /// </summary>
        /// <param name = "sender">The event sender.</param>
        /// <param name = "e">The event args containing the DataGram data.</param>
        public delegate void XDMessageHandler(object sender, XDMessageEventArgs e);

        #endregion

        private readonly XDMessagingClient client;

        public IXDListener GetListenerForMode(XDTransportMode transportMode)
        {
            var listener = Container.Resolve<IXDListener>(Convert.ToString(transportMode));
            if (listener == null)
            {
                throw new NotSupportedException(
                    string.Format(
                        "No concrete IXDListener for mode {0} could be loaded. Install the {0} assembly in the program directory.",
                        transportMode));
            }
            if (transportMode != XDTransportMode.RemoteNetwork)
            {
                var networkListener = Container.Use<IIdentityProvider, MachineNameIdentityProvider>()
                    .Resolve<IXDListener>(Convert.ToString(XDTransportMode.RemoteNetwork));
                if (networkListener != null && networkListener.IsAlive)
                {
                    var networkBroadcaster = client.Broadcasters.GetBroadcasterForMode(transportMode, false);
                    listener = new NetworkRelayListener(networkBroadcaster, listener, networkListener, transportMode);
                }
            }
            return listener;
        }

        #region Constructors and Destructors

        public Listeners(XDMessagingClient client, IocContainer container)
            : base(container)
        {
            Validate.That(client).IsNotNull();

            this.client = client;
        }

        #endregion
    }
}