using System;
using TheCodeKing.Net.Messaging.Helpers;

namespace TheCodeKing.Net.Messaging.Concrete.MultiBroadcast
{
    public class NetworkRelayDataGram
    {
        #region Constants and Fields

        private readonly DataGram dataGram;
        private readonly Guid id;
        private readonly string machineName;

        #endregion

        #region Constructors and Destructors

        public NetworkRelayDataGram()
        {
            dataGram = new DataGram();
        }

        public NetworkRelayDataGram(string machineName, Guid id, string channel, string message)
        {
            this.machineName = machineName;
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

        public static implicit operator DataGram(NetworkRelayDataGram gram)
        {
            return gram.dataGram;
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return string.Concat(MachineName, ":", Id, ":", Channel, ":", Message);
        }

        #endregion

        #region Methods

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