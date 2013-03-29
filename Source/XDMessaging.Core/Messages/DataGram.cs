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
using System.Runtime.Serialization;
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Messages
{
    /// <summary>
    /// 	The data class that is passed between AppDomain boundaries. This is
    /// 	sent as a delimited string containing the channel and message.
    /// </summary>
    [DataContract]
    public class DataGram
    {
        #region Constants and Fields

        private const string VERSION = "1.1";
        private string assemblyQualifiedName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// 	Constructor which creates the data gram from a message and channel name.
        /// </summary>
        /// <param name = "channel">The channel through which the message will be sent.</param>
        /// <param name = "assemblyQualifiedName">The data type used for deserialization hints.</param>
        /// <param name = "message">The string message to send.</param>
        public DataGram(string channel, string assemblyQualifiedName, string message)
        {
            Validate.That(channel, "channel").IsNotNullOrEmpty();
            Validate.That(assemblyQualifiedName, "assemblyQualifiedName").IsNotNullOrEmpty();
            Validate.That(message, "message").IsNotNullOrEmpty();

            Channel = channel;
            AssemblyQualifiedName = assemblyQualifiedName;
            Message = message;
            Version = VERSION;
        }

        internal DataGram()
        {
            Version = VERSION;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 	Gets the AssemblyQualifiedName for the encapsulated message.
        /// </summary>
        [DataMember(Name = "type")]
        public string AssemblyQualifiedName
        {
            get
            {
                if (Version == "1.0")
                {
                    throw new NotSupportedException(
                        "Not supported by this DataGram instance. Upgrade the broadcaster instance to a later version.");
                }
                return assemblyQualifiedName;
            }
            set { assemblyQualifiedName = value; }
        }

        /// <summary>
        /// 	Gets the DataGram version.
        /// </summary>
        [DataMember(Name = "ver")]
        public string Version { get; protected set; }

        /// <summary>
        /// 	Gets the channel name.
        /// </summary>
        [DataMember(Name = "channel")]
        public string Channel { get; protected set; }

        /// <summary>
        /// 	Gets the message.
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; protected set; }

        /// <summary>
        /// 	Indicates whether the DataGram contains valid data.
        /// </summary>
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Channel) && !string.IsNullOrEmpty(Message); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 	Converts the instance to the string delimited format.
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