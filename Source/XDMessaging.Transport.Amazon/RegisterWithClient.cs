using TheCodeKing.Utils.IoC;
using XDMessaging.Fluent;
using XDMessaging.Transport.Amazon;

namespace XDMessaging
{
    public static class RegisterWithClient
    {
        #region Public Methods

        public static IXDBroadcaster GetAmazonBroadcaster(this Broadcasters client)
        {
            
            return client.Container.Resolve<XDAmazonBroadcaster>();
        }

        public static IXDListener GetAmazonListener(this Listeners client)
        {
            return client.Container.Resolve<XDAmazonListener>();
        }

        public static XDMessagingClient WithAmazonUniqueKey(this XDMessagingClient client, string uniqueAppKey)
        {
            var settings = GetSettings();
            settings.UniqueAppKey = uniqueAppKey;
            return client;
        }

        public static XDMessagingClient WithAmazonSettings(this XDMessagingClient client, string accessKey, string accessSecret)
        {
            var settings = GetSettings();
            settings.AccessKey = accessKey;
            settings.SecretKey = accessSecret;
            return client;
        }

        public static XDMessagingClient WithAmazonSettings(this XDMessagingClient client, RegionEndPoint awsRegionEndPoint)
        {
            var settings = GetSettings();
            settings.AwsRegionEndPoint = awsRegionEndPoint;
            return client;
        }

        public static XDMessagingClient WithAmazonSettings(this XDMessagingClient client, string accessKey, string accessSecret, RegionEndPoint awsRegionEndPoint)
        {
            return client.WithAmazonSettings(accessKey, accessSecret).WithAmazonSettings(awsRegionEndPoint);
        }

        public static bool HasValidAmazonSettings(this XDMessagingClient client)
        {
            var settings = GetSettings();
            return settings.IsValid;
        }

        internal static AmazonAccountSettings GetSettings()
        {
            return AmazonAccountSettings.GetInstance();
        }

        #endregion
    }
}