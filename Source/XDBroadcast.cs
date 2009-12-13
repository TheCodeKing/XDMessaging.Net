/*=============================================================================
*
*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expresed or implied.
*
*-----------------------------------------------------------------------------
*	History:
*		11/02/2007	Michael Carlisle				Version 1.0
*       08/09/2007  Michael Carlisle                Version 1.1
*       12/12/2009  Michael Carlisle                Version 2.0
 *                  Added XDIOStream implementation which can be used from Windows Services.
*=============================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using TheCodeKing.Net.Messaging.Concrete.IOStream;
using TheCodeKing.Net.Messaging.Concrete.WindowsMessaging;

namespace TheCodeKing.Net.Messaging
{
    /// <summary>
    /// Class used to broadcast messages to other applications listening
    /// on a particular channel.
    /// </summary>
    public static class XDBroadcast
    {
        /// <summary>
        /// Creates a concrete instance of IXDBroadcast used to broadcast messages to 
        /// other processes.
        /// </summary>
        /// <param name="mode">The transport mechanism to use for interprocess communication.</param>
        /// <returns>The concreate instance of IXDBroadcast</returns>
        public static IXDBroadcast CreateBroadcast(XDTransportMode mode)
        {
            switch (mode)
            {
                case XDTransportMode.IOStream:
                    return new XDIOStream();
                default:
                    return new XDWindowsMessaging();
            }
        }

        /// <summary>
        /// This method is deprecated, and uses the WindowsMessaging XDTransportMode implementation of IXDBroadcast. 
        /// </summary>
        /// <param name="channel">The channel name to broadcast on.</param>
        /// <param name="message">The string message data.</param>
        [Obsolete("Create a concrete implementation of IXDBroadcast using CreateBroadcast(XDTransportMode mode), and call SendToChannel on the instance.")]
        public static void SendToChannel(string channel, string message)
        {
            XDWindowsMessaging windowsMessaging = new XDWindowsMessaging();
            windowsMessaging.SendToChannel(channel, message);
        }
    }
}
