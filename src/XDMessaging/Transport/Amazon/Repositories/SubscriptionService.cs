using System;
using System.Collections.Concurrent;
using System.Reflection;
using Amazon.SQS.Model;
using Conditions;
using XDMessaging.Entities.Amazon;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Repositories
{
    internal sealed class SubscriptionService : ISubscriptionService
    {
        private readonly IAmazonSnsFacade amazonSnsFacade;
        private readonly IAmazonSqsFacade amazonSqsFacade;
        private readonly QueuePoller queuePoller;
        private readonly IResourceCounter resourceCounter;
        private readonly ISubscriberRepository subscriberRespoitroy;

        private readonly ConcurrentDictionary<string, SubscriptionInfo> subscriptions =
            new ConcurrentDictionary<string, SubscriptionInfo>(StringComparer.InvariantCultureIgnoreCase);

        private bool isDisposed;

        public SubscriptionService(IResourceCounter resourceCounter, IAmazonSnsFacade amazonSnsFacade,
            IAmazonSqsFacade amazonSqsFacade, ISubscriberRepository subscriberRespoitroy, QueuePoller queuePoller)
        {
            resourceCounter.Requires("resourceCounter").IsNotNull();
            amazonSnsFacade.Requires("amazonSnsFacade").IsNotNull();
            amazonSqsFacade.Requires("amazonSqsFacade").IsNotNull();
            queuePoller.Requires("queuePoller").IsNotNull();
            subscriberRespoitroy.Requires("subscriberRespoitroy").IsNotNull();

            this.resourceCounter = resourceCounter;
            this.amazonSnsFacade = amazonSnsFacade;
            this.amazonSqsFacade = amazonSqsFacade;
            this.subscriberRespoitroy = subscriberRespoitroy;
            this.queuePoller = queuePoller;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool IsSubscribed(Topic topic, Subscriber subscriber)
        {
            topic.Requires("topic").IsNotNull();
            subscriber.Requires("subscriber").IsNotNull();

            if (isDisposed)
            {
                throw new ObjectDisposedException(MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                    "instance has been disposed");
            }

            var key = GetKey(topic, subscriber);
            return subscriptions.ContainsKey(key);
        }

        public SubscriptionInfo Subscribe(Topic topic, Subscriber subscriber, Action<Message> messageHandler)
        {
            topic.Requires("topic").IsNotNull();
            subscriber.Requires("subscriber").IsNotNull();
            messageHandler.Requires("messageHandler").IsNotNull();

            if (isDisposed)
            {
                throw new ObjectDisposedException(MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                    "instance has been disposed");
            }

            var key = GetKey(topic, subscriber);
            return subscriptions.AddOrUpdate(key, k => AddSubscription(topic, subscriber, messageHandler),
                (x, s) => UpdateSubscription(s, messageHandler));
        }

        public SubscriptionInfo Unsubscribe(Topic topic, Subscriber subscriber)
        {
            topic.Requires("topic").IsNotNull();
            subscriber.Requires("subscriber").IsNotNull();

            if (isDisposed)
            {
                throw new ObjectDisposedException(MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                    "instance has been disposed");
            }

            var key = GetKey(topic, subscriber);
            SubscriptionInfo subscriptionInfo;
            if (subscriptions.TryRemove(key, out subscriptionInfo))
            {
                subscriptionInfo.CancelToken.Cancel();
                if (!subscriber.LongLived || resourceCounter.Decrement(subscriptionInfo.Subscriber.Name) == 0)
                {
                    amazonSnsFacade.UnsubscribeQueueFromTopic(subscriptionInfo.SubscriptionArn);
                }
                subscriptionInfo.CancelToken = null;
                subscriptionInfo.SubscriptionArn = null;
            }
            return subscriptionInfo;
        }

        private static string GetKey(Topic topic, Subscriber subscriber)
        {
            return string.Concat(topic.Name, "-", subscriber.Name);
        }

        public void Publish(Topic topic, string subject, string message)
        {
            topic.Requires("topic").IsNotNull();
            subject.Requires("subject").IsNotNullOrWhiteSpace();
            message.Requires("message").IsNotNullOrWhiteSpace();

            if (isDisposed)
            {
                throw new ObjectDisposedException(MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                    "instance has been disposed");
            }

            amazonSnsFacade.PublishMessageToTopic(topic.TopicArn, subject, message);
        }

        private SubscriptionInfo AddSubscription(Topic topic, Subscriber subscriber, Action<Message> messageHandler)
        {
            var subscriptionInfo = new SubscriptionInfo
            {
                Topic = topic,
                Subscriber = subscriber
            };

            amazonSqsFacade.SetSqsPolicyForSnsPublish(subscriber.QueueUrl, subscriber.QueueArn, topic.TopicArn);
            subscriptionInfo.SubscriptionArn =
                amazonSnsFacade.SubscribeQueueToTopic(subscriptionInfo.Subscriber.QueueArn,
                    subscriptionInfo.Topic.TopicArn);
            if (subscriptionInfo.Subscriber.LongLived)
            {
                resourceCounter.Increment(subscriptionInfo.Subscriber.Name);
            }
            subscriptionInfo.CancelToken = queuePoller.Start(subscriptionInfo, messageHandler);

            return subscriptionInfo;
        }

        private void Dispose(bool disposeManaged)
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            if (!disposeManaged)
            {
                return;
            }

            foreach (var subscription in subscriptions.Values)
            {
                subscription.CancelToken?.Cancel(true);

                if (subscription.IsSubscribed &&
                    (!subscription.Subscriber.LongLived || resourceCounter.Decrement(subscription.Subscriber.Name) == 0))
                {
                    amazonSnsFacade.UnsubscribeQueueFromTopic(subscription.SubscriptionArn);
                }
                subscriberRespoitroy.ExpireSubscriber(subscription.Subscriber);
            }
        }

        private SubscriptionInfo UpdateSubscription(SubscriptionInfo subscriptionInfo, Action<Message> messageHandler)
        {
            if (subscriptionInfo.IsSubscribed)
            {
                return subscriptionInfo;
            }
            subscriptionInfo.SubscriptionArn =
                amazonSnsFacade.SubscribeQueueToTopic(subscriptionInfo.Subscriber.QueueArn,
                    subscriptionInfo.Topic.TopicArn);
            subscriptionInfo.CancelToken = queuePoller.Start(subscriptionInfo, messageHandler);
            return subscriptionInfo;
        }

        ~SubscriptionService()
        {
            Dispose(false);
        }
    }
}