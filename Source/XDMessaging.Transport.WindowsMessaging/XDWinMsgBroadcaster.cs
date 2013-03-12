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
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;

namespace XDMessaging.Transport.WindowsMessaging
{
    /// <summary>
    /// 	The implementation of IXDBroadcast used to broadcast messages acorss appDomain and process boundaries
    /// 	using the XDTransportMode.WindowsMessaging implementation. Non-form based application are not supported.
    /// </summary>
    [XDBroadcasterHint(XDTransportMode.HighPerformanceUI)]
// ReSharper disable InconsistentNaming
    public sealed class XDWinMsgBroadcaster : IXDBroadcaster
// ReSharper restore InconsistentNaming
    {
        #region Constants and Fields

        private readonly ISerializer serializer;

        #endregion

        #region Constructors and Destructors

        private XDWinMsgBroadcaster(ISerializer serializer)
        {
            Validate.That(serializer).IsNotNull();

            this.serializer = serializer;
        }

        #endregion

        /// <summary>
        /// 	Is this instance capable
        /// </summary>
        public bool IsAlive
        {
            get { return true; }
        }

        #region Implemented Interfaces

        #region IXDBroadcaster

        public void SendToChannel(string channelName, object message)
        {
            Validate.That(channelName).IsNotNullOrEmpty();
            Validate.That(message).IsNotNull();

            SendToChannel(channelName, serializer.Serialize(message));
        }

        public void SendToChannel(string channelName, string message)
        {
            Validate.That(channelName).IsNotNullOrEmpty();
            Validate.That(message).IsNotNullOrEmpty();

            // create a DataGram instance, and ensure memory is freed
            using (var dataGram = new WinMsgDataGram(serializer, channelName, message))
            {
                // Allocate the DataGram to a memory address contained in COPYDATASTRUCT
                Native.COPYDATASTRUCT dataStruct = dataGram.ToStruct();
                // Use a filter with the EnumWindows class to get a list of windows containing
                // a property name that matches the destination channel. These are the listening
                // applications.
                var filter = new WindowEnumFilter(XDWinMsgListener.GetChannelKey(channelName));
                var winEnum = new WindowsEnum(filter.WindowFilterHandler);
                foreach (var hWnd in winEnum.Enumerate())
                {
                    IntPtr outPtr;
                    // For each listening window, send the message data. Return if hang or unresponsive within 1 sec.
                    Native.SendMessageTimeout(hWnd, Native.WM_COPYDATA, IntPtr.Zero, ref dataStruct,
                                              Native.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out outPtr);
                }
            }
        }

        #endregion

        #endregion
    }
}