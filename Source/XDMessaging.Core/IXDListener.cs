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
using XDMessaging.Entities;

namespace XDMessaging
{
    /// <summary>
    /// 	The API defined for receiving messages broadcast from a particular implementation
    /// 	of IXDBroadcast. Listeners can register to received messages on particular pseudo channels.
    /// </summary>
    public interface IXDListener : IDisposable
    {
        #region Events

        /// <summary>
        /// 	The event dispatched when new messages a re received. This contains the broadcast message data.
        /// </summary>
        event Listeners.XDMessageHandler MessageReceived;

        #endregion

        #region Public Methods

        /// <summary>
        /// 	Is this instance capable
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// 	Registers the implementation to receive messages on a particular channel.
        /// </summary>
        /// <param name = "channelName">The channel to be registered.</param>
        void RegisterChannel(string channelName);

        /// <summary>
        /// 	Removes the listener from a particular channel. Messages broadcast to channelName will no 
        /// 	longer be received.
        /// </summary>
        /// <param name = "channelName">The channel to be unregistered.</param>
        void UnRegisterChannel(string channelName);

        #endregion
    }
}