using System;
using Conditions;
using XDMessaging.Serialization;

namespace XDMessaging.Transport.WindowsMessaging
{
    // ReSharper disable once InconsistentNaming
    public sealed class XDWinMsgBroadcaster : IXDBroadcaster
    {
        private readonly ISerializer serializer;

        internal XDWinMsgBroadcaster(ISerializer serializer)
        {
            serializer.Requires("serializer").IsNotNull();

            this.serializer = serializer;
        }

        public void SendToChannel(string channelName, object message)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();
            message.Requires("message").IsNotNull();

            SendToChannel(channelName, message.GetType().AssemblyQualifiedName, serializer.Serialize(message));
        }

        public void SendToChannel(string channelName, string message)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();
            message.Requires("message").IsNotNullOrWhiteSpace();

            SendToChannel(channelName, typeof (string).AssemblyQualifiedName, message);
        }

        public bool IsAlive => true;

        private void SendToChannel(string channelName, string dataType, string message)
        {
            channelName.Requires().IsNotNullOrWhiteSpace();
            message.Requires().IsNotNullOrWhiteSpace();

            using (var dataGram = new WinMsgDataGram(serializer, channelName, dataType, message))
            {
                var dataStruct = dataGram.ToStruct();
                var filter = new WindowEnumFilter(XDWinMsgListener.GetChannelKey(channelName));
                var winEnum = new WindowsEnum(filter.WindowFilterHandler);
                foreach (var hWnd in winEnum.Enumerate())
                {
                    IntPtr outPtr;
                    Native.SendMessageTimeout(hWnd, Native.WM_COPYDATA, IntPtr.Zero, ref dataStruct,
                        Native.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out outPtr);
                }
            }
        }
    }
}