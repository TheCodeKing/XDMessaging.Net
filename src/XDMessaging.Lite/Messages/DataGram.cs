using System;
using System.Runtime.Serialization;
using Conditions;

namespace XDMessaging.Messages
{
    [DataContract]
    public class DataGram
    {
        private const string AppVersion = "1.1";
        private string assemblyQualifiedName;

        public DataGram(string channel, string assemblyQualifiedName, string message)
        {
            channel.Requires("channel").IsNotNullOrWhiteSpace();
            assemblyQualifiedName.Requires("assemblyQualifiedName").IsNotNullOrWhiteSpace();
            message.Requires("message").IsNotNullOrWhiteSpace();

            Channel = channel;
            AssemblyQualifiedName = assemblyQualifiedName;
            Message = message;
            Version = AppVersion;
        }

        internal DataGram()
        {
            Version = AppVersion;
        }

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

        [DataMember(Name = "channel")]
        public string Channel { get; protected set; }

        public bool IsValid => !string.IsNullOrEmpty(Channel) && !string.IsNullOrEmpty(Message);

        [DataMember(Name = "message")]
        public string Message { get; protected set; }

        [DataMember(Name = "ver")]
        public string Version { get; protected set; }

        public override string ToString()
        {
            return string.Concat(Channel, ":", Message);
        }
    }
}