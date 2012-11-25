using TheCodeKing.Utils.IoC;
using XDMessaging.Fluent;
using XDMessaging.Transport.IOStream;

namespace XDMessaging
{
    public static class RegisterWithClient
    {
        #region Public Methods

        public static IXDBroadcaster GetIoStreamBroadcaster(this Broadcasters client)
        {
            return client.Container.Resolve<XDIoStreamBroadcaster>();
        }

        public static IXDListener GetIoStreamListener(this Listeners client)
        {
            return client.Container.Resolve<XDIoStreamListener>();
        }

        #endregion
    }
}