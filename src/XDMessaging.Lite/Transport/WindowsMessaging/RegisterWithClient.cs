using XDMessaging.Entities;

// ReSharper disable once CheckNamespace
namespace XDMessaging
{
    public static partial class RegisterWithClient
    {
        public static IXDBroadcaster GetWindowsMessagingBroadcaster(this Broadcasters client)
        {
            return client.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);
        }

        public static IXDListener GetWindowsMessagingListener(this Listeners client)
        {
            return client.GetListenerForMode(XDTransportMode.HighPerformanceUI);
        }
    }
}