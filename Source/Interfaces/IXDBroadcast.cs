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
    /// The API defined for dispatching messages interprocess and across appDomains.
    /// </summary>
    public interface IXDBroadcast
    {
        /// <summary>
        /// The API used to broadcast messages to a channel, and other applications that
        /// may be listening.
        /// </summary>
        /// <param name="channel">The channel name to broadcast on.</param>
        /// <param name="message">The string message data.</param>
        void SendToChannel(string channel, string message);
    }
}
