using System;
using System.Configuration;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Core;
using XDMessaging.Core.IoC;
using XDMessaging.Core.Specialized;

namespace XDMessaging.Transport.Amazon
{
    [TransportModeHint(XDTransportMode.RemoteNetwork)]
    public sealed class XDAmazonListener : IXDListener
    {
        #region Constants and Fields

        private readonly IAmazonFacade amazonFacade;
        private readonly AmazonAccountSettings amazonAccountSettings;
        private readonly ISerializer serializer;
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        static XDAmazonListener()
        {
            var instance = SimpleIoCContainerBootstrapper.GetInstance();
            instance.Register<ISerializer, SpecializedSerializer>();
            instance.Register(() => ConfigurationManager.AppSettings);
            instance.Register(AmazonAccountSettings.GetInstance);
            instance.Register<IAmazonFacade, AmazonFacade>();
        }

        public XDAmazonListener(ISerializer serializer, IAmazonFacade amazonFacade, AmazonAccountSettings amazonAccountSettings)
        {
            Validate.That(serializer).IsNotNull();
            Validate.That(amazonFacade).IsNotNull();
            Validate.That(amazonAccountSettings).IsNotNull();

            this.serializer = serializer;
            this.amazonFacade = amazonFacade;
            this.amazonAccountSettings = amazonAccountSettings;
        }

        /// <summary>
        ///   Deconstructor, cleans unmanaged resources only
        /// </summary>
        ~XDAmazonListener()
        {
            Dispose(false);
        }

        #endregion

        #region Events

        public event XDListener.XDMessageHandler MessageReceived;

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        ///   Dispose implementation, which ensures the native window is destroyed
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IXDListener

        public void RegisterChannel(string channelName)
        {
            Validate.That(channelName).IsNotNull();

            var queueName = NameHelper.GetQueueNameFromChannel(amazonAccountSettings.UniqueAppKey, channelName);
            var topicName = NameHelper.GetTopicNameFromChannel(amazonAccountSettings.UniqueAppKey, channelName);

            // get topic ARN
            var topicArn = amazonFacade.CreateOrRetrieveTopic(topicName);
            // create queue for subscriber
            string queueArn;
            var queueUrl = amazonFacade.CreateOrRetrieveQueue(queueName, out queueArn);
            // enable SNS to publish to the SQS
            amazonFacade.SetSqsPolicyForSnsPublish(queueUrl, topicArn);
            // subscribe subscriber to topic
            amazonFacade.SubscribeQueueToTopic(queueArn, topicArn);

        }

        public void UnRegisterChannel(string channelName)
        {
            Validate.That(channelName).IsNotNull();
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///   Dispose implementation which ensures the native window is destroyed, and
        ///   managed resources detached.
        /// </summary>
        private void Dispose(bool disposeManaged)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposeManaged)
                {
                    if (MessageReceived != null)
                    {
                        // remove all handlers
                        Delegate[] del = MessageReceived.GetInvocationList();
                        foreach (XDListener.XDMessageHandler msg in del)
                        {
                            MessageReceived -= msg;
                        }
                    }
                }
            }
        }

        #endregion
    }
}