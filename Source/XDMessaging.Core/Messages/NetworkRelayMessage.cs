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
using System;
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Messages
{
    /// <summary>
    ///   The data struct that is passed between AppDomain boundaries for the NetworkRelay
    ///   implementation. This is sent as a delimited string containing the channel and message.
    /// </summary>
    /// <summary>
    ///   Class used to demostrate sending objects via the XDMessaging library.
    /// </summary>
    [Serializable]
    public class NetworkRelayMessage
    {
        #region Constants and Fields

        private readonly string machineName;
        private readonly XDTransportMode originatingTransportMode;
        private readonly string channel;
        private readonly string message;

        #endregion

        #region Constructors and Destructors

        public NetworkRelayMessage(string machineName, XDTransportMode originatingTransportMode, string channel, string message)
        {
            Validate.That(machineName).IsNotNullOrEmpty();
            Validate.That(channel).IsNotNullOrEmpty();
            Validate.That(message).IsNotNullOrEmpty();

            this.machineName = machineName;
            this.originatingTransportMode = originatingTransportMode;
            this.channel = channel;
            this.message = message;
        }

        #endregion

        #region Properties

        public XDTransportMode OriginatingTransportMode
        {
            get { return originatingTransportMode; }
        }

        public string Message
        {
            get { return message; }
        }

        public string Channel
        {
            get { return channel; }
        }

        public string MachineName
        {
            get { return machineName; }
        }

        #endregion
    }
}