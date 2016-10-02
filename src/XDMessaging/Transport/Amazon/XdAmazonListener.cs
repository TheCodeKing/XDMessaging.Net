using System;
using Amazon.SQS.Model;
using Conditions;
using XDMessaging.Entities;
using XDMessaging.Entities.Amazon;
using XDMessaging.IdentityProviders;
using XDMessaging.Messages;
using XDMessaging.Serialization;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon
{
    // ReSharper disable once InconsistentNaming
    internal sealed class XDAmazonListener : IXDListener
    {
        private readonly object disposeLock = new object();
        private readonly ISerializer serializer;
        private readonly ISubscriberRepository subscriberRepository;
        private readonly ISubscriptionService subscriptionService;
        private readonly ITopicRepository topicRepository;
        private readonly string uniqueInstanceId;
        private readonly bool useLongLiveQueues;
        private bool disposed;

        internal XDAmazonListener(IIdentityProvider identityProvider, ISerializer serializer,
            ITopicRepository topicRepository,
            ISubscriberRepository subscriberRepository, ISubscriptionService subscriptionService)
        {
            identityProvider.Requires("identityProvider").IsNotNull();
            serializer.Requires("serializer").IsNotNull();
            topicRepository.Requires("topicRepository").IsNotNull();
            subscriberRepository.Requires("subscriberRepository").IsNotNull();
            subscriptionService.Requires("subscriptionService").IsNotNull();

            useLongLiveQueues = identityProvider.Scope == IdentityScope.Machine;
            uniqueInstanceId = identityProvider.GetUniqueId();
            this.serializer = serializer;
            this.topicRepository = topicRepository;
            this.subscriberRepository = subscriberRepository;
            this.subscriptionService = subscriptionService;
        }

        public event Listeners.XDMessageHandler MessageReceived;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RegisterChannel(string channelName)
        {
            channelName.Requires("channelName").IsNotNull();

            if (disposed)
            {
                throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
                }

                var topic = topicRepository.GetTopic(channelName);
                var subscriber = subscriberRepository.GetSubscriber(channelName, uniqueInstanceId,
                    useLongLiveQueues);

                subscriptionService.Subscribe(topic, subscriber, OnMessageReceived);
            }
        }

        public void UnRegisterChannel(string channelName)
        {
            channelName.Requires("channelName").IsNotNull();

            if (disposed)
            {
                throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IXDListener", "This instance has been disposed.");
                }

                var topic = topicRepository.GetTopic(channelName);
                var subscriber = subscriberRepository.GetSubscriber(channelName, uniqueInstanceId,
                    useLongLiveQueues);

                subscriptionService.Unsubscribe(topic, subscriber);
            }
        }

        public bool IsAlive => AmazonAccountSettings.GetInstance().IsValid;

        private void Dispose(bool disposeManaged)
        {
            if (disposed)
            {
                return;
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                subscriptionService.Dispose();
                if (!disposeManaged || MessageReceived == null)
                {
                    return;
                }

                var del = MessageReceived.GetInvocationList();
                foreach (var item in del)
                {
                    var msg = (Listeners.XDMessageHandler) item;
                    MessageReceived -= msg;
                }
            }
        }

        private void OnMessageReceived(Message message)
        {
            if (disposed)
            {
                return;
            }

            lock (disposeLock)
            {
                if (disposed)
                {
                    return;
                }

                var notification = serializer.Deserialize<AmazonSqsNotification>(message.Body);
                var dataGram = serializer.Deserialize<DataGram>(notification.Message);

                var topic = topicRepository.GetTopic(dataGram.Channel);
                var subscriber = subscriberRepository.GetSubscriber(dataGram.Channel, uniqueInstanceId);
                var isSubscribed = subscriptionService.IsSubscribed(topic, subscriber);
                if (!disposed && dataGram.IsValid && MessageReceived != null && isSubscribed)
                {
                    MessageReceived(this, new XDMessageEventArgs(dataGram));
                }
            }
        }

        ~XDAmazonListener()
        {
            Dispose(false);
        }
    }
}