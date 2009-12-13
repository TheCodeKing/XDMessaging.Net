/*=============================================================================
*
*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expresed or implied.
*
*-----------------------------------------------------------------------------
*	History:
*		11/02/2007	Michael Carlisle				Version 1.0
*       08/09/2007  Michael Carlisle                Version 1.1
*       12/12/2009  Michael Carlisle                Version 2.0
 *                  Added XDIOStream implementation which can be used from Windows Services.
*=============================================================================
*/
using System;
using System.Windows.Forms;
using TheCodeKing.Net.Messaging.Concrete.IOStream;
using TheCodeKing.Net.Messaging.Concrete.WindowsMessaging;

namespace TheCodeKing.Net.Messaging
{
    /// <summary>
    /// An implementation of IXDListener used to send and recieve messages interprocess, using the Windows
    /// Messaging XDTransportMode. Applications may leverage this instance to register listeners on pseudo 'channels', and 
    /// receive messages broadcast using a concrete IXDBroadcast implementation on the same machine. Non-form based 
    /// application are not supported by this implementation.
    /// </summary>
    public sealed class XDListener : NativeWindow, IXDListener
    {
        /// <summary>
        /// Creates a concrete IXDListener which uses the XDTransportMode.WindowsMessaging implementaion. This method
        /// is now deprecated and XDListener.CreateInstance(XDTransportMode.WindowsMessaging) should be used instead.
        /// </summary>
        [Obsolete("Use the static factory method CreateListener to create a particular implementation of IXDListener.")]
        public XDListener()
            :this(true)
        {
        }

        /// <summary>
        /// The non-obsolete constructor used internally for creating new instances of XDListener.
        /// </summary>
        /// <param name="nonObsolete"></param>
        internal XDListener(bool nonObsolete)
        {
            CreateParams p = new CreateParams();
            p.Width = 0;
            p.Height = 0;
            p.X = 0;
            p.Y = 0;
            p.Style = (int)Native.WS_CHILD;
            p.Caption = Guid.NewGuid().ToString();
            p.Parent = Native.GetDesktopWindow();
            base.CreateHandle(p);
        }

        /// <summary>
        /// The delegate used for handling cross AppDomain communications.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args containing the DataGram data.</param>
        public delegate void XDMessageHandler(object sender, XDMessageEventArgs e);

        /// <summary>
        /// The event fired when messages are received.
        /// </summary>
        public event XDMessageHandler MessageReceived;

        /// <summary>
        /// Creates an concrete implementation of IXDListener to listen for messages using
        /// either a specific XDTransportMode.
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public static IXDListener CreateListener(XDTransportMode transport)
        {
            switch (transport)
            {
                case XDTransportMode.IOStream:
                    return new XDIOStream();
                default:
                    return new XDListener(true);
            }
        }

        /// <summary>
        /// Registers the instance to recieve messages from a named channel.
        /// </summary>
        /// <param name="channelName">The channel name to listen on.</param>
        public void RegisterChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(channelName, "The channel name cannot be null or empty.");
            }
            Native.SetProp(this.Handle, GetChannelKey(channelName), (int)this.Handle);
        }
        /// <summary>
        /// Unregisters the channel name with the instance, so that messages from this 
        /// channel will no longer be received.
        /// </summary>
        /// <param name="channelName">The channel name to stop listening for.</param>
        public void UnRegisterChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(channelName, "The channel name cannot be null or empty.");
            }
            Native.RemoveProp(this.Handle, GetChannelKey(channelName));
        }
        /// <summary>
        /// The native window message filter used to catch our custom WM_COPYDATA
        /// messages containing cross AppDomain messages. All other messages are ignored.
        /// </summary>
        /// <param name="msg">A representation of the native Windows Message.</param>
        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg);
            if (msg.Msg == Native.WM_COPYDATA)
            {
                if (MessageReceived != null)
                {
                    DataGram dataGram = DataGram.FromPointer(msg.LParam);
                    if (!string.IsNullOrEmpty(dataGram.Message))
                    {
                        MessageReceived(this, new XDMessageEventArgs(dataGram));
                    }
                }
            }
        }
        /// <summary>
        /// Gets a channel key string associated with the channel name. This is used as the 
        /// property name attached to listening windows in order to identify them as
        /// listeners. Using the key instead of user defined channel name avoids protential 
        /// property name clashes. 
        /// </summary>
        /// <param name="channelName">The channel name for which a channel key is required.</param>
        /// <returns>The string channel key.</returns>
        internal static string GetChannelKey(string channelName)
        {
            return string.Format("TheCodeKing.Net.XDServices.{0}", channelName);
        }

    }
}
