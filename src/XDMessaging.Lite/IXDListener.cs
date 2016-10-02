using System;
using XDMessaging.Entities;

namespace XDMessaging
{
    // ReSharper disable once InconsistentNaming
    public interface IXDListener : IDisposable
    {
        bool IsAlive { get; }
        event Listeners.XDMessageHandler MessageReceived;

        void RegisterChannel(string channelName);

        void UnRegisterChannel(string channelName);
    }
}