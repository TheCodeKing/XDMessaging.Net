using TheCodeKing.Utils.IoC;
using XDMessaging.Fluent;
using XDMessaging.Transport.WindowsMessaging;

namespace XDMessaging
{
    public static class RegisterWithClient
    {
        #region Public Methods

        public static IXDBroadcaster GetWindowsMessagingBroadcaster(this Broadcasters client)
        {
            return client.Container.Resolve<XDWinMsgBroadcaster>();
        }

        public static IXDListener GetWindowsMessagingListener(this Listeners client)
        {
            return client.Container.Resolve<XDWinMsgListener>();
        }

        #endregion
    }
}