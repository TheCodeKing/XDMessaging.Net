using System;
using System.Collections.Concurrent;
using System.Configuration;
using Amazon.SQS.Model;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Core;
using XDMessaging.Core.IoC;
using XDMessaging.Core.Message;
using XDMessaging.Core.Specialized;

namespace XDMessaging.Transport.Amazon
{
    [TransportModeHint(XDTransportMode.RemoteNetwork)]
    public sealed class XDAmazonListener : IXDListener
    {
        #region Constants and Fields

        private readonly AmazonAccountSettings amazonAccountSettings;
        private readonly IAwsQueueReceiver awsQueueReceiver;
        private readonly IAmazonFacade amazonFacade;

        private readonly ConcurrentDictionary<string, SubscriptionInfo> registeredChannels =
            new ConcurrentDictionary<string, SubscriptionInfo>(StringComparer.InvariantCultureIgnoreCase);

        private readonly ISerializer serializer;
        private readonly string uniqueInstanceId = Guid.NewGuid().ToString("N");
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        static XDAmazonListener()
        {
            var instance = SimpleIoCContainerBootstrapper.GetInstance();
            instance.Register<ISerializer, SpecializedSerializer>();
            instance.Register<IAwsQueueReceiver, AwsQueueReceiver>();
            instance.Register(() => ConfigurationManager.AppSettings);
            instance.Register(AmazonAccountSettings.GetInstance);
            instance.Register<IAmazonFacade, AmazonFacade>();
        }

        public XDAmazonListener(ISerializer serializer, IAmazonFacade amazonFacade,
                                AmazonAccountSettings amazonAccountSettings, IAwsQueueReceiver awsQueueReceiver)
        {
            Validate.That(serializer).IsNotNull();
            Validate.That(amazonFacade).IsNotNull();
            Validate.That(amazonAccountSettings).IsNotNull();
            Validate.That(awsQueueReceiver).IsNotNull();

            this.serializer = serializer;
            this.amazonFacade = amazonFacade;
            this.amazonAccountSettings = amazonAccountSettings;
            this.awsQueueReceiver = awsQueueReceiver;
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

            if (disposed)
            {
                return;
            }
            registeredChannels.AddOrUpdate(channelName, CreateChannelListener, EnsureChannelListening);
        }


        public void UnRegisterChannel(string channelName)
        {
            Validate.That(channelName).IsNotNull();

            if (disposed)
            {
                return;
            }
            if (registeredChannels.ContainsKey(channelName))
            {
                registeredChannels.AddOrUpdate(channelName, CreateChannelListener, EnsureChannelNotListening);   
            }
        }

        #endregion

        #endregion

        #region Methods

        private void OnMessageReceived(Message message)
        {
            var notification = serializer.Deserialize<AmazonSqsNotification>(message.Body);
            var dataGram = serializer.Deserialize<DataGram>(notification.Message);
            if (!disposed && dataGram.IsValid && MessageReceived != null)
            {
                MessageReceived(this, new XDMessageEventArgs(dataGram));
            }
        }

        private SubscriptionInfo CreateChannelListener(string channelName)
        {
            var topicName = NameHelper.GetTopicNameFromChannel(amazonAccountSettings.UniqueAppKey, channelName);
            var queueName = NameHelper.GetQueueNameFromListenerId(amazonAccountSettings.UniqueAppKey, channelName, uniqueInstanceId);

            // get topic ARN
            var topicArn = amazonFacade.CreateOrRetrieveTopic(topicName);
            // create queue for subscriber
            string queueArn;
            var queueUrl = amazonFacade.CreateOrRetrieveQueue(queueName, out queueArn);
            // enable SNS to publish to the SQS
            amazonFacade.SetSqsPolicyForSnsPublish(queueUrl, queueArn, topicArn);

            var subscription = new SubscriptionInfo
                       {
                           ChannelName = channelName,
                           QueueUrl = queueUrl,
                           QueueArn = queueArn,
                           TopicArn = topicArn,
                           TopicName = topicName
                       };
            EnsureChannelListening(channelName, subscription);
            return subscription;
        }

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
                        var del = MessageReceived.GetInvocationList();
                        foreach (XDListener.XDMessageHandler msg in del)
                        {
                            MessageReceived -= msg;
                        }
                    }
                }
                foreach (var queueSubscription in registeredChannels.Values)
                {
                    EnsureChannelNotListening(queueSubscription.ChannelName, queueSubscription);
                    amazonFacade.DeleteQueue(queueSubscription.QueueUrl);
                }
                registeredChannels.Clear();
            }
        }

        private SubscriptionInfo EnsureChannelListening(string channelName, SubscriptionInfo subscriptionInfo)
        {
            if (!subscriptionInfo.IsSubscribed)
            {
                subscriptionInfo.SubscriptionArn = amazonFacade.SubscribeQueueToTopic(subscriptionInfo.QueueArn,
                                                                                      subscriptionInfo.TopicArn);
                awsQueueReceiver.Start(subscriptionInfo, OnMessageReceived);
            }
            return subscriptionInfo;
        }

        private SubscriptionInfo EnsureChannelNotListening(string channelName, SubscriptionInfo subscriptionInfo)
        {
            if (subscriptionInfo.IsSubscribed)
            {
                amazonFacade.UnsubscribeQueueToTopic(subscriptionInfo.SubscriptionArn);
                subscriptionInfo.SubscriptionArn = null;
                awsQueueReceiver.Stop(subscriptionInfo);
            }
            return subscriptionInfo;
        }

        #endregion
    }
}