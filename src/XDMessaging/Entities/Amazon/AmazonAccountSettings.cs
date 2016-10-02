using System;
using System.Collections.Specialized;
using System.Configuration;
using Conditions;

namespace XDMessaging.Entities.Amazon
{
    internal sealed class AmazonAccountSettings
    {
        private const string AwsAccessKey = "AWSAccessKey";
        private const string AwsSecretKey = "AWSSecretKey";
        private const string DefaultKey = "XDM";

        private static readonly Lazy<AmazonAccountSettings> Instance =
            new Lazy<AmazonAccountSettings>(() => new AmazonAccountSettings(ConfigurationManager.AppSettings));

        public AmazonAccountSettings(NameValueCollection appSettings)
        {
            appSettings.Requires("appSettings").IsNotNull();

            RegionEndPoint = null;
            UniqueAppKey = DefaultKey;
            AccessKey = appSettings[AwsAccessKey];
            SecretKey = appSettings[AwsSecretKey];
        }

        public string AccessKey { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(SecretKey);

        public RegionEndPoint? RegionEndPoint { get; set; }

        public string SecretKey { get; set; }

        public string UniqueAppKey { get; set; }

        public static AmazonAccountSettings GetInstance()
        {
            return Instance.Value;
        }
    }
}