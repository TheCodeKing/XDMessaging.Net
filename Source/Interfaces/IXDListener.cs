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

namespace TheCodeKing.Net.Messaging
{
    /// <summary>
    /// The API defined for receiving messages broadcast from a particular implementation
    /// of IXDBroadcast. Listeners can register to received messages on particular pseudo channels.
    /// </summary>
    public interface IXDListener : IDisposable
    {
        /// <summary>
        /// The event dispatched when new messages a re received. This contains the broadcast message data.
        /// </summary>
        event XDListener.XDMessageHandler MessageReceived;

        /// <summary>
        /// Registers the implementation to receive messages on a particular channel.
        /// </summary>
        /// <param name="channelName">The channel to be registered.</param>
        void RegisterChannel(string channelName);

        /// <summary>
        /// Removes the listener from a particular channel. Messages broadcast to channelName will no 
        /// longer be received.
        /// </summary>
        /// <param name="channelName">The channel to be unregistered.</param>
        void UnRegisterChannel(string channelName);
    }
}
