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
using System.Collections.Generic;
using XDMessaging.Entities;
using XDMessaging.Specialized;

// ReSharper disable CheckNamespace

namespace XDMessaging
// ReSharper restore CheckNamespace
{
    public static class RegisterWithClient
    {
        #region Public Methods

        public static IXDBroadcaster GetMulticastBroadcaster(this Broadcasters client,
                                                             params IXDBroadcaster[] broadcasters)
        {
            return new MulticastBroadcaster(broadcasters);
        }

        public static IXDBroadcaster GetMulticastBroadcaster(this Broadcasters client,
                                                             IEnumerable<IXDBroadcaster> broadcasters)
        {
            return new MulticastBroadcaster(broadcasters);
        }

        #endregion
    }
}