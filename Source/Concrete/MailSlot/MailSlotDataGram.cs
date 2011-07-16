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
using System.Linq;
using System.Text;
using TheCodeKing.Net.Messaging.Helpers;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    /// <summary>
    ///   The data struct that is passed between AppDomain boundaries for the MailSlot
    ///   implementation. This is sent as a delimited string containing the channel and message.
    /// </summary>
    internal sealed class MailSlotDataGram
    {
        #region Constants and Fields

        private readonly string channel;
        private readonly Guid id;
        private readonly ushort index;
        private readonly ushort total;
        private readonly string[] fragments;

        #endregion

        #region Constructors and Destructors

        public MailSlotDataGram(Guid id, ushort index, ushort total, string channel, string message)
        {
            if (index > total)
            {
                throw new ArgumentException("index cannot be greater then total", "index");
            }
            this.id = id;
            this.index = index;
            this.total = total;
            this.channel = channel;
            fragments = new string[total];
            fragments[index] = message;
        }

        internal MailSlotDataGram()
        {
            fragments = new string[0];
        }

        #endregion

        #region Properties

        public bool IsComplete
        {
            get { return !fragments.Contains(null); }
        }

        /// <summary>
        ///   Gets the channel name.
        /// </summary>
        public string Channel
        {
            get { return channel; }
        }

        /// <summary>
        ///   Gets the fragment index of this message.
        /// </summary>
        public ushort Index
        {
            get { return index; }
        }

        /// <summary>
        ///   Gets the number of fragments in this message.
        /// </summary>
        public ushort TotalParts
        {
            get { return total; }
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
            get 
            {
                return string.Join("", fragments); 
            }
        }

        /// <summary>
        ///   Gets the unique message Id.
        /// </summary>
        public string UniqueId
        {
            get { return (id == Guid.Empty) ? string.Empty : string.Concat(id, "-", index, "-of-", total); }
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
            return new DataGram(dataGram.Channel, dataGram.Message);
        }

        #endregion

        #region Public Methods

        public byte[] ToBytes()
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(ToString());
        }

        /// <summary>
        ///   Converts the instance to the string delimited format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(Id, ":", index, ":", total, ":", Channel, ":", Message);
        }

        public void AddFragment(ushort msgIndex, string message)
        {
            fragments[msgIndex] = message;
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
                string[] parts = rawmessage.Split(new[] {':'}, 5);
                if (parts.Length == 5)
                {
                    Guid guid;
                    GuidHelper.TryParse(parts[0], out guid);
                    ushort index;
                    ushort total;
                    if (ushort.TryParse(parts[1], out index) && ushort.TryParse(parts[2], out total))
                    {
                        return new MailSlotDataGram(guid, index, total, parts[3], parts[4]);
                    }
                }
            }
            return new MailSlotDataGram();
        }

        #endregion
    }
}