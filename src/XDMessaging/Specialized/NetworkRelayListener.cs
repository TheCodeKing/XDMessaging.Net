using System;
using System.Threading;
using System.Threading.Tasks;
using Conditions;
using XDMessaging.Entities;
using XDMessaging.Messages;

namespace XDMessaging.Specialized
{
    internal sealed class NetworkRelayListener : IXDListener
    {
        private const int NetworkReTryTimeoutMilliSeconds = 10000;
        private readonly IXDBroadcaster nativeBroadcast;
        private readonly IXDListener nativeListener;
        private readonly IXDListener propagateListener;
        private bool disposed;

        internal NetworkRelayListener(IXDBroadcaster nativeBroadcast,
            IXDListener nativeListener,
            IXDListener propagateListener, XDTransportMode mode)
        {
            nativeBroadcast.Requires("nativeBroadcast").IsNotNull();
            nativeListener.Requires("nativeListener").IsNotNull();
            propagateListener.Requires("propagateListener").IsNotNull();

            this.nativeBroadcast = nativeBroadcast;
            this.propagateListener = propagateListener;
            this.nativeListener = nativeListener;
            this.nativeListener.MessageReceived += OnMessageReceived;
            RegisterNetworkListener(mode);
        }

        public event Listeners.XDMessageHandler MessageReceived;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RegisterChannel(string channelName)
        {
            if (disposed)
            {
                return;
            }

            nativeListener.RegisterChannel(channelName);
        }

        public void UnRegisterChannel(string channelName)
        {
            if (disposed)
            {
                return;
            }

            nativeListener.UnRegisterChannel(channelName);
        }

        public bool IsAlive => propagateListener.IsAlive;

        public void Dispose(bool disposeManaged)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            propagateListener?.Dispose();

            if (!disposeManaged)
            {
                return;
            }

            nativeListener?.Dispose();
        }

        private void OnMessageReceived(object sender, XDMessageEventArgs e)
        {
            if (MessageReceived != null)
            {
                MessageReceived(sender, e);
            }
        }

        private void OnNetworkMessageReceived(object sender, XDMessageEventArgs e)
        {
            if (disposed || !e.DataGram.IsValid)
            {
                return;
            }

            TypedDataGram<NetworkRelayMessage> dataGram = e.DataGram;
            if (dataGram != null && dataGram.IsValid && dataGram.Message.MachineName != Environment.MachineName)
            {
                // rebroadcast locally
                Task.Factory.StartNew(
                    () => nativeBroadcast.SendToChannel(dataGram.Message.Channel, dataGram.Message.Message));
            }
        }

        private void RegisterNetworkListener(XDTransportMode mode)
        {
            if (disposed)
            {
                return;
            }

            if (!IsAlive)
            {
                return;
            }

            Task.Factory.StartNew(() =>
            {
                propagateListener.RegisterChannel(
                    NetworkRelayBroadcaster.GetNetworkListenerName(mode));
                propagateListener.MessageReceived += OnNetworkMessageReceived;
            }).ContinueWith(t =>
            {
                // ReSharper disable once UnusedVariable
                var e = t.Exception;
                if (disposed)
                {
                    return;
                }

                Thread.Sleep(NetworkReTryTimeoutMilliSeconds);
                RegisterNetworkListener(mode);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        ~NetworkRelayListener()
        {
            Dispose(false);
        }
    }
}