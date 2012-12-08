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
using TheCodeKing.Utils.IoC;
using XDMessaging.Entities;
using XDMessaging.Transport.IOStream;

namespace XDMessaging
{
    public static class RegisterWithClient
    {
        #region Public Methods

        public static IXDBroadcaster GetIoStreamBroadcaster(this Broadcasters client)
        {
            return client.Container.Resolve<XDIOStreamBroadcaster>();
        }

        public static IXDListener GetIoStreamListener(this Listeners client)
        {
            return client.Container.Resolve<XDIoStreamListener>();
        }

        #endregion
    }
}