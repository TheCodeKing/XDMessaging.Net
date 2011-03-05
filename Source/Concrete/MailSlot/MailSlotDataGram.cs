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

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    /// <summary>
    ///   The data struct that is passed between AppDomain boundaries for the MailSlot
    ///   implementation. This is sent as a delimited string containing the channel and message.
    /// </summary>
    internal class MailSlotDataGram
    {
        #region Constants and Fields

        private readonly DataGram dataGram;
        private readonly Guid id;

        #endregion

        #region Constructors and Destructors

        public MailSlotDataGram(Guid id, string channel, string message)
        {
            this.id = id;
            dataGram = new DataGram(channel, message);
        }

        internal MailSlotDataGram()
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
            get { return !string.IsNullOrEmpty(Channel) && !string.IsNullOrEmpty(Message) && Id != Guid.Empty; }
        }

        #endregion

        #region Operators

        /// <summary>
        ///   Allows implicit casting from MailSlotDataGram to DataGram.
        /// </summary>
        /// <param name = "dataGram"></param>
        /// <returns></returns>
        public static implicit operator DataGram(MailSlotDataGram dataGram)
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
            return string.Concat(Id, ":", Channel, ":", Message);
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Creates an instance of DataGram from a raw delimited string.
        /// </summary>
        /// <param name = "rawmessage"></param>
        /// <returns></returns>
        internal static MailSlotDataGram ExpandFromRaw(string rawmessage)
        {
            // if the message contains valid data
            if (!string.IsNullOrEmpty(rawmessage) && rawmessage.Contains(":"))
            {
                // extract the channel name and message data
                string[] parts = rawmessage.Split(new[] {':'}, 3);
                if (parts.Length == 3)
                {
                    Guid guid;
                    GuidHelper.TryParse(parts[0], out guid);
                    return new MailSlotDataGram(guid, parts[1], parts[2]);
                }
            }
            return new MailSlotDataGram();
        }

        #endregion
    }
}