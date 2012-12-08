/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using TheCodeKing.Utils.IoC;
using XDMessaging.Entities;
using XDMessaging.Transport.Amazon;
using XDMessaging.Transport.Amazon.Entities;

// ReSharper disable CheckNamespace
namespace XDMessaging
// ReSharper restore CheckNamespace
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
            settings.RegionEndPoint = awsRegionEndPoint;
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