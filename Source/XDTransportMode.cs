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
    /// This defines the tranport modes that can be used for inter-process communication.
    /// </summary>
    public enum XDTransportMode
    {
        /// <summary>
        /// Uses Windows Messaging to pass data beteen processes using WM_COPYDATA. This mode
        /// is ideal for performant communication between windows form based applications.
        /// </summary>
        WindowsMessaging,
        /// <summary>
        /// Uses file watchers to trigger events and pass data between processes. This mode can 
        /// be used within non-form based applications such as Windows Services.
        /// </summary>
        IOStream
    }
}
