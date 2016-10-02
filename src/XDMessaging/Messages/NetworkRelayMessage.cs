using System;
using Conditions;

namespace XDMessaging.Messages
{
    [Serializable]
    public class NetworkRelayMessage
    {
        public NetworkRelayMessage(string machineName, XDTransportMode originatingTransportMode, string channel,
            string message)
        {
            machineName.Requires("machineName").IsNotNullOrWhiteSpace();
            channel.Requires("channel").IsNotNullOrWhiteSpace();
            message.Requires("message").IsNotNullOrWhiteSpace();

            MachineName = machineName;
            OriginatingTransportMode = originatingTransportMode;
            Channel = channel;
            Message = message;
        }

        public string Channel { get; }

        public string MachineName { get; }

        public string Message { get; }

        public XDTransportMode OriginatingTransportMode { get; }
    }
}