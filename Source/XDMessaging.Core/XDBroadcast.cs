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
        #region Constants and Fields

        #endregion

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
        /// <param name = "mode">The broadcast mode.</param>
        /// <param name = "propagateNetwork">true to propagate messages over the local network.</param>
        /// <returns></returns>
        public static IXDBroadcast CreateBroadcast(XDTransportMode mode, bool propagateNetwork)
        {
            if (mode == XDTransportMode.RemoteNetwork)
            {
                return CreateBroadcast(mode);
            }
            var broadcast = CreateBroadcast(mode);
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
        /// <param name = "modes">One or more transport mechanisms to use for interprocess communication.</param>
        /// <returns>The concreate instance of IXDBroadcast</returns>
        public static IXDBroadcast CreateBroadcast(params XDTransportMode[] modes)
        {
            if (modes.Length == 0)
            {
                throw new ArgumentException("At least one transport mode must be defined.");
            }
            if (modes.Length == 1)
            {
                return Container.Resolve<IXDBroadcast>(Convert.ToString(modes[0]));
            }

            // ensure only one of each type added
            var singleItems = new Dictionary<XDTransportMode, IXDBroadcast>();
            foreach (var mode in modes)
            {
                // only add one of each mode
                if (!singleItems.ContainsKey(mode))
                {
                    singleItems.Add(mode, Container.Resolve<IXDBroadcast>(Convert.ToString(mode)));
                }
            }
            return new XDMultiBroadcast(singleItems.Values);
        }

        #endregion
    }
}