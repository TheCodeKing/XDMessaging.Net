using System;
using TheCodeKing.Net.Messaging.Helpers;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    internal class MailSlotDataGram
    {
        #region Constants and Fields

        private readonly DataGram dataGram;
        private readonly Guid id;

        #endregion

        #region Constructors and Destructors

        public MailSlotDataGram()
        {
            dataGram = new DataGram();
        }

        public MailSlotDataGram(Guid id, string channel, string message)
        {
            this.id = id;
            dataGram = new DataGram(channel, message);
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

        public static implicit operator DataGram(MailSlotDataGram gram)
        {
            return gram.dataGram;
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return string.Concat(Id, ":", Channel, ":", Message);
        }

        #endregion

        #region Methods

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