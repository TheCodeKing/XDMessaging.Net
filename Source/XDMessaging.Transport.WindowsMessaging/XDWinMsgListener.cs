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
using System.Windows.Forms;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Entities;

namespace XDMessaging.Transport.WindowsMessaging
{
    /// <summary>
    /// 	An implementation of IXDListener used to send and recieve messages interprocess, using the Windows
    /// 	Messaging XDTransportMode. Applications may leverage this instance to register listeners on pseudo 'channels', and 
    /// 	receive messages broadcast using a concrete IXDBroadcast implementation on the same machine. Non-form based 
    /// 	application are not supported by this implementation.
    /// </summary>
    [XDListenerHint(XDTransportMode.HighPerformanceUI)]
// ReSharper disable InconsistentNaming
    public sealed class XDWinMsgListener : NativeWindow, IXDListener
// ReSharper restore InconsistentNaming
    {
        #region Constants and Fields

        private readonly ISerializer serializer;
        private bool disposed;
        private readonly object disposeLock = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// 	The constructor used internally for creating new instances of XDListener.
        /// </summary>
        internal XDWinMsgListener(ISerializer serializer)
        {
            Validate.That(serializer).IsNotNull();

            this.serializer = serializer;

            // create a top-level native window
            var p = new CreateParams
                        {
                            Width = 0,
                            Height = 0,
                            X = 0,
                            Y = 0,
                            Caption = string.Concat("TheCodeKing.Net.XDServices.", Guid.NewGuid().ToString()),
                            Parent = IntPtr.Zero
                        };

            CreateHandle(p);
        }

        /// <summary>
        /// 	Deconstructor, cleans unmanaged resources only
        /// </summary>
        ~XDWinMsgListener()
        {
            Dispose(false);
        }

        #endregion

        #region Events

        /// <summary>
        /// 	The event fired when messages are received.
        /// </summary>
        public event Listeners.XDMessageHandler MessageReceived;

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// 	Dispose implementation, which ensures the native window is destroyed
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IXDListener

        /// <summary>
        /// Is this instance capable 
        /// </summary>
        public bool IsAlive
        {
            get { return true; }
        }

        /// <summary>
        /// 	Registers the instance to recieve messages from a named channel.
        /// </summary>
        /// <param name = "channelName">The channel name to listen on.</param>
        public void RegisterChannel(string channelName)
        {
            Validate.That(channelName).IsNotNullOrEmpty();

            if (!disposed)
            {
                lock (disposeLock)
                {
                    if (!disposed)
                    {
                        Native.SetProp(Handle, GetChannelKey(channelName), (int)Handle);
                        return;
                    }
                }
            }
            throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
        }

        /// <summary>
        /// 	Unregisters the channel name with the instance, so that messages from this 
        /// 	channel will no longer be received.
        /// </summary>
        /// <param name = "channelName">The channel name to stop listening for.</param>
        public void UnRegisterChannel(string channelName)
        {
            Validate.That(channelName).IsNotNullOrEmpty();

            if (!disposed)
            {
                lock (disposeLock)
                {
                    if (!disposed)
                    {
                        Native.RemoveProp(Handle, GetChannelKey(channelName));
                        return;
                    }
                }
            }
            throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// 	Gets a channel key string associated with the channel name. This is used as the 
        /// 	property name attached to listening windows in order to identify them as
        /// 	listeners. Using the key instead of user defined channel name avoids protential 
        /// 	property name clashes.
        /// </summary>
        /// <param name = "channelName">The channel name for which a channel key is required.</param>
        /// <returns>The string channel key.</returns>
        internal static string GetChannelKey(string channelName)
        {
            return string.Format("TheCodeKing.Net.XDServices.{0}", channelName);
        }

        /// <summary>
        /// 	The native window message filter used to catch our custom WM_COPYDATA
        /// 	messages containing cross AppDomain messages. All other messages are ignored.
        /// </summary>
        /// <param name = "msg">A representation of the native Windows Message.</param>
        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg);
            if (msg.Msg != Native.WM_COPYDATA)
            {
                return;
            }
            // we can free any unmanaged resources immediately in the dispose, managed channel and message 
            // data will still be retained in the object passed to the event
            using (var dataGram = WinMsgDataGram.FromPointer(msg.LParam, serializer))
            {
                if (MessageReceived != null && dataGram.IsValid)
                {
                    MessageReceived.Invoke(this, new XDMessageEventArgs(dataGram));
                }
            }
        }

        /// <summary>
        /// 	Dispose implementation which ensures the native window is destroyed, and
        /// 	managed resources detached.
        /// </summary>
        private void Dispose(bool disposeManaged)
        {
            if (!disposed)
            {
                lock (disposeLock)
                {
                    if (!disposed)
                    {
                        disposed = true;
                        if (disposeManaged)
                        {
                            if (MessageReceived != null)
                            {
                                // remove all handlers
                                Delegate[] del = MessageReceived.GetInvocationList();
                                foreach (Listeners.XDMessageHandler msg in del)
                                {
                                    MessageReceived -= msg;
                                }
                            }
                            if (Handle != IntPtr.Zero)
                            {
                                DestroyHandle();
                                Dispose();
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}