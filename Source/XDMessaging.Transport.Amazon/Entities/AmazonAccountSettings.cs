/*=============================================================================
*
*	(C) Copyright 2013, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using System;
using System.Collections.Specialized;
using System.Configuration;
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Transport.Amazon.Entities
{
    internal sealed class AmazonAccountSettings
    {
        #region Constants and Fields

        private const string defaultKey = "XDM";
        private const string awsAccessKey = "AWSAccessKey";
        private const string awsSecretKey = "AWSSecretKey";

        private static readonly Lazy<AmazonAccountSettings> instance =
            new Lazy<AmazonAccountSettings>(() => new AmazonAccountSettings(ConfigurationManager.AppSettings));

        #endregion

        #region Constructors and Destructors

        public AmazonAccountSettings(NameValueCollection appSettings)
        {
            Validate.That(appSettings).IsNotNull();

            RegionEndPoint = null;
            UniqueAppKey = defaultKey;
            AccessKey = appSettings[awsAccessKey];
            SecretKey = appSettings[awsSecretKey];
        }

        #endregion

        #region Properties

        public string AccessKey { get; set; }

        public RegionEndPoint? RegionEndPoint { get; set; }

        public string SecretKey { get; set; }

        public string UniqueAppKey { get; set; }

        #endregion

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(SecretKey); }
        }

        #region Public Methods

        public static AmazonAccountSettings GetInstance()
        {
            return instance.Value;
        }

        #endregion
    }
}