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
using System.Collections.Generic;
using System.Linq;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;
using XDMessaging.Core.IoC;
using XDMessaging.Core.Specialized;

namespace XDMessaging.Core
{
    /// <summary>
    ///   Class used to broadcast messages to other applications listening
    ///   on a particular channel.
    /// </summary>
    public static class XDBroadcast
    {
        #region Constructors and Destructors

        static XDBroadcast()
        {
            Container = SimpleIoCContainerBootstrapper.GetInstance();
        }

        #endregion

        #region Properties

        public static IocContainer Container { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Creates an instance of IXDBroadcast with the otption to propagate over the local network.
        /// </summary>
        /// <param name = "transportMode">The broadcast mode.</param>
        /// <param name = "propagateNetwork">true to propagate messages over the local network.</param>
        /// <returns></returns>
        public static IXDBroadcast CreateBroadcast(XDTransportMode transportMode, bool propagateNetwork)
        {
            var broadcast = Container.Resolve<IXDBroadcast>(Convert.ToString(transportMode));
            if (broadcast == null)
            {
                throw new NotSupportedException(
                    string.Format(
                        "No concrete IXDBroadcast for mode {0} could be loaded. Install the {0} assembly in the program directory.",
                        transportMode));
            }
            if (transportMode == XDTransportMode.RemoteNetwork)
            {
                return broadcast;
            }
            /*if (propagateNetwork)
            {
                var networkRelay = new NetworkRelayBroadcast(serializer,
                                                             CreateBroadcast(XDTransportMode.RemoteNetwork), mode);
                broadcast = new XDMultiBroadcast(broadcast, networkRelay);
            }*/
            return broadcast;
        }

        /// <summary>
        ///   Creates a concrete instance of IXDBroadcast used to broadcast messages to 
        ///   other processes in one or more modes.
        /// </summary>
        /// <param name = "transportModes">One or more transport mechanisms to use for interprocess communication.</param>
        /// <returns>The concreate instance of IXDBroadcast</returns>
        public static IXDBroadcast CreateBroadcast(params XDTransportMode[] transportModes)
        {
            Validate.That(transportModes, "At least one transport mode must be defined.").ContainsGreaterThan(0);

            if (transportModes.Length == 1)
            {
                return CreateBroadcast(transportModes[0], false);
            }

            // ensure only one of each type added
            var singleItems = new Dictionary<XDTransportMode, IXDBroadcast>();
            foreach (var mode in transportModes.Where(mode => !singleItems.ContainsKey(mode)))
            {
                singleItems.Add(mode, CreateBroadcast(transportModes[0], false));
            }
            return new XDMultiBroadcast(singleItems.Values);
        }

        #endregion
    }
}