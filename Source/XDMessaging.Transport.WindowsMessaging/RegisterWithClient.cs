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
using TheCodeKing.Utils.IoC;
using XDMessaging.Entities;
using XDMessaging.Transport.WindowsMessaging;

// ReSharper disable CheckNamespace
namespace XDMessaging
// ReSharper restore CheckNamespace
{
    public static class RegisterWithClient
    {
        #region Public Methods

        public static IXDBroadcaster GetWindowsMessagingBroadcaster(this Broadcasters client)
        {
            return client.Container.Resolve<XDWinMsgBroadcaster>();
        }

        public static IXDListener GetWindowsMessagingListener(this Listeners client)
        {
            return client.Container.Resolve<XDWinMsgListener>();
        }

        #endregion
    }
}