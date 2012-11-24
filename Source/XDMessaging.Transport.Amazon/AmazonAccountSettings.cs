using System;
using System.Collections.Specialized;
using System.Configuration;
using Amazon;
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Transport.Amazon
{
    public sealed class AmazonAccountSettings
    {
        #region Constants and Fields

        private static readonly Lazy<AmazonAccountSettings> instance = new Lazy<AmazonAccountSettings>(() => new AmazonAccountSettings(ConfigurationManager.AppSettings));

        private const string awsAccessKey = "AWSAccessKey";
        private const string awsSecretKey = "AWSSecretKey";

        #endregion

        #region Constructors and Destructors

        public AmazonAccountSettings(NameValueCollection appSettings)
        {
            Validate.That(appSettings).IsNotNull();

            RegionEndPoint = null;
            UniqueAppKey = "XDM";
            AccessKey = appSettings[awsAccessKey];
            SecretKey = appSettings[awsSecretKey];
        }

        public RegionEndpoint RegionEndPoint { get; set; }

        public string SecretKey { get; set; }

        public string AccessKey { get; set; }

        public string UniqueAppKey { get; set; }

        public static AmazonAccountSettings GetInstance()
        {
            return instance.Value;
        }

        #endregion


        public void Configure(string amazonAccessKey, string amazonSecretKey)
        {
            Configure(amazonSecretKey, amazonSecretKey, null);
        }

        public void Configure(string amazonAccessKey, string amazonSecretKey, RegionEndpoint regionEndpoint)
        {
            Validate.That(amazonAccessKey).IsNotNullOrEmpty();
            Validate.That(amazonSecretKey).IsNotNullOrEmpty();

            AccessKey = amazonAccessKey;
            SecretKey = amazonSecretKey;

            RegionEndPoint = regionEndpoint;
        }
    }
}