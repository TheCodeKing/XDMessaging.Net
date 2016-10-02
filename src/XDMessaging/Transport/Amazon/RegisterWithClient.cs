using XDMessaging.Entities;
using XDMessaging.Entities.Amazon;

// ReSharper disable once CheckNamespace
namespace XDMessaging
{
    public static partial class RegisterWithClient
    {
        public static IXDBroadcaster GetAmazonBroadcaster(this Broadcasters client)
        {
            return client.GetBroadcasterForMode(XDTransportMode.RemoteNetwork);
        }

        public static IXDListener GetAmazonListener(this Listeners client)
        {
            return client.GetListenerForMode(XDTransportMode.RemoteNetwork);
        }

        public static bool HasValidAmazonSettings(this XDMessagingClient client)
        {
            var settings = GetAmazonSettings();
            return settings.IsValid;
        }

        public static XDMessagingClient WithAmazonSettings(this XDMessagingClient client, string accessKey,
            string accessSecret)
        {
            var settings = GetAmazonSettings();
            settings.AccessKey = accessKey;
            settings.SecretKey = accessSecret;
            return client;
        }

        public static XDMessagingClient WithAmazonSettings(this XDMessagingClient client,
            RegionEndPoint awsRegionEndPoint)
        {
            var settings = GetAmazonSettings();
            settings.RegionEndPoint = awsRegionEndPoint;
            return client;
        }

        public static XDMessagingClient WithAmazonSettings(this XDMessagingClient client, string accessKey,
            string accessSecret, RegionEndPoint awsRegionEndPoint)
        {
            return client.WithAmazonSettings(accessKey, accessSecret).WithAmazonSettings(awsRegionEndPoint);
        }

        public static XDMessagingClient WithAmazonUniqueKey(this XDMessagingClient client, string uniqueAppKey)
        {
            var settings = GetAmazonSettings();
            settings.UniqueAppKey = uniqueAppKey;
            return client;
        }

        internal static AmazonAccountSettings GetAmazonSettings()
        {
            return AmazonAccountSettings.GetInstance();
        }
    }
}