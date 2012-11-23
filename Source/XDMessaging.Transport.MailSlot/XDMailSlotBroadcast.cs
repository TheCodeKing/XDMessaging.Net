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
using XDMessaging.Core.Interfaces;

namespace XDMessaging.Core.Concrete.MailSlot
{
    /// <summary>
    ///   Implementation of IXDBroadcast which sends messages over the local network and 
    ///   interprocess. Messages sent to a named channel will be received by the first available
    ///   instance of IXDListener in the same mode. Each message may only be read by ONE listener
    ///   on a single machine. Other listeners will queue until they can take ownership of the listener
    ///   singleton.
    /// </summary>
    public sealed class XDMailSlotBroadcast : IXDBroadcast
    {
        #region Constants and Fields

        internal const string GlobalScope = "*";
        internal static readonly string LocalScope = Environment.MachineName;

        private const int maxMailSlotSize = 424;

        /// <summary>
        ///   Indicates the base path of the MailSlot.
        /// </summary>
        private const string slotLocation = @"\mailslot\xdmessaging\";

        /// <summary>
        ///   The writer to use for sending messages over MailSlot.
        /// </summary>
        private readonly string mailSlotScope;

        private readonly MailSlotWriter mailSlotWriter;
        private readonly ISerializerHelper serializerHelper;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Internal constructor.
        /// </summary>
        internal XDMailSlotBroadcast(ISerializerHelper serializerHelper, bool propagateNetwork)
        {
            if (serializerHelper == null)
            {
                throw new ArgumentNullException("serializerHelper");
            }
            this.serializerHelper = serializerHelper;

            mailSlotScope = propagateNetwork
                                ? GlobalScope
                                : LocalScope;

            mailSlotWriter = new MailSlotWriter();
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcast

        public void SendToChannel(string channelName, object message)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentException("The channel name must be defined", "channelName");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message", "The messsage packet cannot be null");
            }
            if (channelName.Contains(":"))
            {
                throw new ArgumentException("The channel name may not contain the ':' character.", "channelName");
            }

            SendToChannel(channelName, serializerHelper.Serialize(message));
        }

        /// <summary>
        ///   Implementation of IXDBroadcast for sending messages to a named channel on the local network.
        /// </summary>
        /// <param name = "channelName"></param>
        /// <param name = "message">The message.</param>
        public void SendToChannel(string channelName, string message)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentException("The channel name must be defined", "channelName");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message", "The messsage packet cannot be null");
            }
            if (channelName.Contains(":"))
            {
                throw new ArgumentException("The channel name may not contain the ':' character.", "channelName");
            }

            // the message is broken into fragements due to the mailslot size limitation of 424.
            var fragmentor = new NetworkMessageFragmentor(channelName, message);
            var messages = fragmentor.GetFragments(maxMailSlotSize);
            var mailSlotLocation = GetChannelPath(mailSlotScope, channelName);

            foreach (var fragment in messages)
            {
                mailSlotWriter.Write(mailSlotLocation, fragment.ToBytes());
            }
        }

        #endregion

        #endregion

        #region Methods

        internal static string GetChannelPath(string scope, string channelName)
        {
            return string.Concat(@"\\", scope, slotLocation, channelName);
        }

        #endregion
    }
}