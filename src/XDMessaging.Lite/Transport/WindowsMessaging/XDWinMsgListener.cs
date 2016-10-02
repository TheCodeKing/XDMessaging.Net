using System;
using System.Windows.Forms;
using Conditions;
using XDMessaging.Entities;
using XDMessaging.Serialization;

namespace XDMessaging.Transport.WindowsMessaging
{
    // ReSharper disable once InconsistentNaming
    internal sealed class XDWinMsgListener : NativeWindow, IXDListener
    {
        private readonly object disposeLock = new object();
        private readonly ISerializer serializer;
        private bool disposed;

        internal XDWinMsgListener(ISerializer serializer)
        {
            serializer.Requires("serializer").IsNotNull();

            this.serializer = serializer;

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

        public event Listeners.XDMessageHandler MessageReceived;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RegisterChannel(string channelName)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();

            if (disposed)
            {
                throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
                }

                Native.SetProp(Handle, GetChannelKey(channelName), (int) Handle);
            }
        }

        public void UnRegisterChannel(string channelName)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();

            if (disposed)
            {
                throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
                }

                Native.RemoveProp(Handle, GetChannelKey(channelName));
            }
        }

        public bool IsAlive => true;

        internal static string GetChannelKey(string channelName)
        {
            return $"TheCodeKing.Net.XDServices.{channelName}";
        }

        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg);
            if (msg.Msg != Native.WM_COPYDATA)
            {
                return;
            }

            using (var dataGram = WinMsgDataGram.FromPointer(msg.LParam, serializer))
            {
                if (MessageReceived != null && dataGram.IsValid)
                {
                    MessageReceived.Invoke(this, new XDMessageEventArgs(dataGram));
                }
            }
        }

        private void Dispose(bool disposeManaged)
        {
            if (disposed)
            {
                return;
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                if (!disposeManaged)
                {
                    return;
                }

                if (MessageReceived != null)
                {
                    var del = MessageReceived.GetInvocationList();
                    foreach (var item in del)
                    {
                        var msg = (Listeners.XDMessageHandler) item;
                        MessageReceived -= msg;
                    }
                }
                if (Handle == IntPtr.Zero)
                {
                    return;
                }

                DestroyHandle();
                Dispose();
            }
        }

        ~XDWinMsgListener()
        {
            Dispose(false);
        }
    }
}