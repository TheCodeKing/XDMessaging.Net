/*=============================================================================
*
*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
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

namespace TheCodeKing.Net.Messaging.Concrete.MultiBroadcast
{
    /// <summary>
    ///   An implementation of IXDBroadcast that encapsulates multiple broadcast instances
    ///   so that messages can be send using multiple modes.
    /// </summary>
    internal sealed class XDMultiBroadcast : IXDBroadcast
    {
        #region Constants and Fields

        /// <summary>
        ///   The list of IXDBraodcast instances used to broadcast from this instance.
        /// </summary>
        private readonly IEnumerable<IXDBroadcast> broadcastInstances;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   The constructor which takes an IEnumerable list of IXDBroadcast instances.
        /// </summary>
        /// <param name = "broadcastInstances"></param>
        internal XDMultiBroadcast(IEnumerable<IXDBroadcast> broadcastInstances)
        {
            this.broadcastInstances = broadcastInstances;
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcast

        /// <summary>
        ///   The implementation of IXDBroadcast used to send messages in 
        ///   multiple modes.
        /// </summary>
        /// <param name = "channel"></param>
        /// <param name = "message"></param>
        public void SendToChannel(string channel, string message)
        {
            foreach (var broadcast in broadcastInstances)
            {
                broadcast.SendToChannel(channel, message);
            }
        }

        #endregion

        #endregion
    }
}