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
using System.Runtime.Serialization;
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Core.Message
{
    /// <summary>
    ///   The data class that is passed between AppDomain boundaries. This is
    ///   sent as a delimited string containing the channel and message.
    /// </summary>
    [DataContract]
    public class DataGram
    {
        #region Constants and Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Constructor which creates the data gram from a message and channel name.
        /// </summary>
        /// <param name = "channel">The channel through which the message will be sent.</param>
        /// <param name = "message">The string message to send.</param>
        public DataGram(string channel, string message)
        {
            Validate.That(channel, "channel").IsNotNullOrEmpty();
            Validate.That(message, "message").IsNotNullOrEmpty();

            this.Channel = channel;
            this.Message = message;
            this.Version = "1.0";
        }

        internal DataGram()
        {
            this.Version = "1.0";
        }

        #endregion

        #region Properties

        [DataMember(Name = "ver")]
        public string Version { get; protected set; }

        /// <summary>
        ///   Gets the channel name.
        /// </summary>
        [DataMember(Name="channel")]
        public string Channel { get; protected set; }

        /// <summary>
        ///   Gets the message.
        /// </summary>
        [DataMember(Name="message")]
        public string Message { get; protected set; }

        /// <summary>
        ///   Indicates whether the DataGram contains valid data.
        /// </summary>
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Channel) && !string.IsNullOrEmpty(Message); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the instance to the string delimited format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(Channel, ":", Message);
        }

        #endregion

        #region Methods


        #endregion
    }
}