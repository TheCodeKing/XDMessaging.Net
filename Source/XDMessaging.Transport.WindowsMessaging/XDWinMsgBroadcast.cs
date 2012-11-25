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
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Core;
using XDMessaging.Core.Specialized;

namespace XDMessaging.Transport.WindowsMessaging
{
    /// <summary>
    ///   The implementation of IXDBroadcast used to broadcast messages acorss appDomain and process boundaries
    ///   using the XDTransportMode.WindowsMessaging implementation. Non-form based application are not supported.
    /// </summary>
    [TransportModeHint(XDTransportMode.HighPerformanceUI)]
    public sealed class XDWinMsgBroadcast : IXDBroadcast
    {
        #region Constants and Fields

        private readonly ISerializer serializer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialize method called from XDMessaging.Core before the instance is constructed.
        /// This allows external classes to registered dependencies with the IocContainer.
        /// </summary>
        /// <param name="container">The IocContainer instance used to construct this class.</param>
        private static void Initialize(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            container.Register<ISerializer, SpecializedSerializer>();
        }

        private XDWinMsgBroadcast(ISerializer serializer)
        {
            Validate.That(serializer).IsNotNull();

            this.serializer = serializer;
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcast

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