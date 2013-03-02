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
using System.Collections.Generic;
using System.Linq;
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Specialized
{
    /// <summary>
    /// 	The implementation used to broadcast messages using multiple TransportModes.
    /// </summary>
    internal sealed class MulticastBroadcaster : IXDBroadcaster
    {
        #region Constants and Fields

        private readonly IEnumerable<IXDBroadcaster> broadcasters;

        #endregion

        #region Constructors and Destructors

        public MulticastBroadcaster(IEnumerable<IXDBroadcaster> broadcasters)
        {
            Validate.That(broadcasters).IsNotNull();
            Validate.That(broadcasters).ContainsGreaterThan(0);

            this.broadcasters = broadcasters;
        }

        public MulticastBroadcaster(params IXDBroadcaster[] broadcasters)
            : this((IEnumerable<IXDBroadcaster>) broadcasters)
        {
        }

        /// <summary>
        /// 	Is this instance capable
        /// </summary>
        public bool IsAlive
        {
            get { return broadcasters.Any(x => x.IsAlive); }
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcaster

        public void SendToChannel(string channel, string message)
        {
            foreach (var broadcaster in broadcasters)
            {
                if (broadcaster.IsAlive)
                {
                    broadcaster.SendToChannel(channel, message);
                }
            }
        }

        public void SendToChannel(string channel, object message)
        {
            foreach (var broadcaster in broadcasters)
            {
                if (broadcaster.IsAlive)
                {
                    broadcaster.SendToChannel(channel, message);
                }
            }
        }

        #endregion

        #endregion
    }
}