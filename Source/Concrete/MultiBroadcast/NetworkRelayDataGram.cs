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
using TheCodeKing.Net.Messaging.Helpers;

namespace TheCodeKing.Net.Messaging.Concrete.MultiBroadcast
{
    /// <summary>
    ///   The data struct that is passed between AppDomain boundaries for the NetworkRelay
    ///   implementation. This is sent as a delimited string containing the channel and message.
    /// </summary>
    internal class NetworkRelayDataGram
    {
        #region Constants and Fields

        /// <summary>
        ///   Stores the basic encapsulated DataGram.
        /// </summary>
        private readonly DataGram dataGram;

        /// <summary>
        ///   A unique Id for the message to prevent duplicates.
        /// </summary>
        private readonly Guid id;

        /// <summary>
        ///   Stores the current machine name.
        /// </summary>
        private readonly string machineName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of NetworkRelayDataGram.
        /// </summary>
        /// <param name = "machineName"></param>
        /// <param name = "id"></param>
        /// <param name = "channel"></param>
        /// <param name = "message"></param>
        public NetworkRelayDataGram(string machineName, Guid id, string channel, string message)
        {
            this.machineName = machineName;
            this.id = id;
            dataGram = new DataGram(channel, message);
        }

        internal NetworkRelayDataGram()
        {
            dataGram = new DataGram();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the channel name.
        /// </summary>
        public string Channel
        {
            get { return dataGram.Channel; }
        }

        /// <summary>
        ///   Gets the message Id.
        /// </summary>
        public Guid Id
        {
            get { return id; }
        }

        /// <summary>
        ///   The name of the machine broadcasting the message.
        /// </summary>
        public string MachineName
        {
            get { return machineName; }
        }

        /// <summary>
        ///   Gets the message.
        /// </summary>
        public string Message
        {
            get { return dataGram.Message; }
        }

        /// <summary>
        ///   Indicates whether the DataGram contains valid data.
        /// </summary>
        internal bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Channel) && !string.IsNullOrEmpty(Message)
                       && Id == Guid.Empty && !string.IsNullOrEmpty(MachineName);
            }
        }

        #endregion

        #region Operators

        /// <summary>
        ///   Allows implicit casting from NetworkRelayDataGram to DataGram.
        /// </summary>
        /// <param name = "dataGram"></param>
        /// <returns></returns>
        public static implicit operator DataGram(NetworkRelayDataGram dataGram)
        {
            return dataGram.dataGram;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Converts the instance to the string delimited format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(MachineName, ":", Id, ":", Channel, ":", Message);
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Creates an instance of DataGram from a raw delimited string.
        /// </summary>
        /// <param name = "rawmessage"></param>
        /// <returns></returns>
        internal static NetworkRelayDataGram ExpandFromRaw(string rawmessage)
        {
            // if the message contains valid data
            if (!string.IsNullOrEmpty(rawmessage) && rawmessage.Contains(":"))
            {
                // extract the channel name and message data
                string[] parts = rawmessage.Split(new[] {':'}, 4);
                if (parts.Length == 4)
                {
                    Guid guid;
                    GuidHelper.TryParse(parts[1], out guid);
                    return new NetworkRelayDataGram(parts[0], guid, parts[2], parts[3]);
                }
            }
            return new NetworkRelayDataGram();
        }

        #endregion
    }
}