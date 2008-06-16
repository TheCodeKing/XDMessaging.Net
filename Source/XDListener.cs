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
*=============================================================================
*/
using System;
using System.Windows.Forms;
using TheCodeKing.Native;

namespace TheCodeKing.Net.Messaging
{
    /// <summary>
    /// A class used to send and recieve cross AppDomain messages. Applications may
    /// leverage this instance to register listeners on pseudo 'channels', and 
    /// broadcast messages to other applications.
    /// </summary>
    public sealed class XDListener : NativeWindow
    {
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
        /// Default constructor. This creates a hidden native window used to 
        /// listen for Windows Messages, and filter out the WM_COPYDATA commands.
        /// </summary>
        public XDListener()
        {
            CreateParams p = new CreateParams();
            p.Width = 0;
            p.Height = 0;
            p.X = 0;
            p.Y = 0;
            p.Style = (int)Win32.WS_CHILD;
            p.Caption = Guid.NewGuid().ToString();
            p.Parent = Win32.GetDesktopWindow();
            base.CreateHandle(p);
        }
        /// <summary>
        /// Registers the instance to recieve messages from a named channel.
        /// </summary>
        /// <param name="channelName">The channel name to listen on.</param>
        public void RegisterChannel(string channelName)
        {
            Win32.SetProp(this.Handle, GetChannelKey(channelName), (int)this.Handle);
        }
        /// <summary>
        /// Unregisters the channel name with the instance, so that messages from this 
        /// channel will no longer be received.
        /// </summary>
        /// <param name="channelName">The channel name to stop listening for.</param>
        public void UnRegisterChannel(string channelName)
        {
            Win32.RemoveProp(this.Handle, GetChannelKey(channelName));
        }
        /// <summary>
        /// The native window message filter used to catch our custom WM_COPYDATA
        /// messages containing cross AppDomain messages. All other messages are ignored.
        /// </summary>
        /// <param name="msg">A representation of the native Windows Message.</param>
        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg);
            if (msg.Msg == Win32.WM_COPYDATA)
            {
                if (MessageReceived == null) return;
                DataGram dataGram = DataGram.FromPointer(msg.LParam);
                MessageReceived(this, new XDMessageEventArgs(dataGram));
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
