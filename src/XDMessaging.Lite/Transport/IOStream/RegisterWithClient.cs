using XDMessaging.Entities;

// ReSharper disable once CheckNamespace
namespace XDMessaging
{
    public static partial class RegisterWithClient
    {
        // ReSharper disable once InconsistentNaming
        public static IXDBroadcaster GetIOStreamBroadcaster(this Broadcasters client)
        {
            return client.GetBroadcasterForMode(XDTransportMode.Compatibility);
        }

        // ReSharper disable once InconsistentNaming
        public static IXDListener GetIOStreamListener(this Listeners client)
        {
            return client.GetListenerForMode(XDTransportMode.Compatibility);
        }
    }
}