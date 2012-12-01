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
using System;
using System.Collections.Concurrent;
using System.Reflection;
using Amazon.SQS.Model;
using TheCodeKing.Utils.Contract;
using XDMessaging.Transport.Amazon.Entities;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Repositories
{
    internal sealed class SubscriptionService : ISubscriptionService
    {
        private readonly ConcurrentDictionary<string, SubscriptionInfo> subscriptions = new ConcurrentDictionary<string, SubscriptionInfo>(StringComparer.InvariantCultureIgnoreCase);
        private bool isDisposed;
        private readonly IAmazonSnsFacade amazonSnsFacade;
        private readonly IAmazonSqsFacade amazonSqsFacade;
        private readonly ISubscriberRepository subscriberRespoitroy;
        private readonly QueuePoller queuePoller;

        public SubscriptionService(IAmazonSnsFacade amazonSnsFacade, IAmazonSqsFacade amazonSqsFacade, ISubscriberRepository subscriberRespoitroy, QueuePoller queuePoller)
        {
            Validate.That(amazonSnsFacade).IsNotNull();
            Validate.That(amazonSqsFacade).IsNotNull();
            Validate.That(queuePoller).IsNotNull();
            Validate.That(subscriberRespoitroy).IsNotNull();

            this.amazonSnsFacade = amazonSnsFacade;
            this.amazonSqsFacade = amazonSqsFacade;
            this.subscriberRespoitroy = subscriberRespoitroy;
            this.queuePoller = queuePoller;
        }

        /// <summary>
        /// Indicates whether the subscriber is subscribed to the topic.
        /// </summary>
        /// <param name="topic">The topic to publish the message to. The topic represents a channel.</param>
        /// <param name="subscriber">The suject used when publishing to the topic.</param>
        /// <returns></returns>
        public bool IsSubscribed(Topic topic, Subscriber subscriber)
        {
            Validate.That(topic).IsNotNull();
            Validate.That(subscriber).IsNotNull();

            if (isDisposed)
            {
                throw new ObjectDisposedException(MethodBase.GetCurrentMethod().DeclaringType.Name, "instance has been disposed");
            }

            var key = GetKey(topic, subscriber);
            return subscriptions.ContainsKey(key);
        }

        /// <summary>
        /// Publish messsage to topic.
        /// </summary>
        /// <param name="topic">The topic to publish the message to. The topic represents a channel.</param>
        /// <param name="subject">The suject used when publishing to the topic.</param>
        /// <param name="message">The body message used when publishing to the topic.</param>
        public void Publish(Topic topic, string subject, string message)
        {
            Validate.That(topic).IsNotNull();
            Validate.That(subject).IsNotNullOrEmpty();
            Validate.That(message).IsNotNullOrEmpty();

            if (isDisposed)
            {
                throw new ObjectDisposedException(MethodBase.GetCurrentMethod().DeclaringType.Name, "instance has been disposed");
            }

            amazonSnsFacade.PublishMessageToTopic(topic.TopicArn, subject, message);
        }

        private static string GetKey(Topic topic, Subscriber subscriber)
        {
            return string.Concat(topic.Name, "-", subscriber.Name);
        }

        /// <summary>
        /// Subscribe subscriber from topic.
        /// </summary>
        /// <param name="topic">The topic to be subscribed to.</param>
        /// <param name="subscriber">The subscriber to be unsubscribed from the topic.</param>
        /// <param name="messageHandler">The delegate for handling the messages.</param>
        /// <returns></returns>
        public SubscriptionInfo Subscribe(Topic topic, Subscriber subscriber, Action<Message> messageHandler)
        {
            Validate.That(topic).IsNotNull();
            Validate.That(subscriber).IsNotNull();
            Validate.That(messageHandler).IsNotNull();

            if (isDisposed)
            {
                throw new ObjectDisposedException(MethodBase.GetCurrentMethod().DeclaringType.Name, "instance has been disposed");
            }

            var key = GetKey(topic, subscriber);
            return subscriptions.AddOrUpdate(key, k => AddSubscription(topic, subscriber, messageHandler), (x,s) => UpdateSubscription(s, messageHandler));
        }

        /// <summary>
        /// Unsubscribe subscriber from topic.
        /// </summary>
        /// <param name="topic">The topic to be unsubscribed from.</param>
        /// <param name="subscriber">The subscriber to be unsubscribed from the topic.</param>
        /// <returns></returns>
        public SubscriptionInfo Unsubscribe(Topic topic, Subscriber subscriber)
        {
            Validate.That(topic).IsNotNull();
            Validate.That(subscriber).IsNotNull();

            if (isDisposed)
            {
                throw new ObjectDisposedException(MethodBase.GetCurrentMethod().DeclaringType.Name, "instance has been disposed");
            }

            var key = GetKey(topic, subscriber);
            SubscriptionInfo subscriptionInfo;
            if (subscriptions.TryRemove(key, out subscriptionInfo))
            {
                subscriptionInfo.CancelToken.Cancel();
                amazonSnsFacade.UnsubscribeQueueFromTopic(subscriptionInfo.SubscriptionArn);
                subscriptionInfo.CancelToken = null;
                subscriptionInfo.SubscriptionArn = null;
            }
            return subscriptionInfo;
        }

        private SubscriptionInfo AddSubscription(Topic topic, Subscriber subscriber, Action<Message> messageHandler)
        {
            var subscriptionInfo = new SubscriptionInfo
            {
                Topic=topic,
                Subscriber=subscriber
            };

            amazonSqsFacade.SetSqsPolicyForSnsPublish(subscriber.QueueUrl, subscriber.QueueArn, topic.TopicArn);
            subscriptionInfo.SubscriptionArn = amazonSnsFacade.SubscribeQueueToTopic(subscriptionInfo.Subscriber.QueueArn, subscriptionInfo.Topic.TopicArn);
            subscriptionInfo.CancelToken = queuePoller.Start(subscriptionInfo, messageHandler);

            return subscriptionInfo;

        }

        private SubscriptionInfo UpdateSubscription(SubscriptionInfo subscriptionInfo, Action<Message> messageHandler)
        {
            if (subscriptionInfo.IsSubscribed)
            {
                return subscriptionInfo;
            }
            subscriptionInfo.SubscriptionArn = amazonSnsFacade.SubscribeQueueToTopic(subscriptionInfo.Subscriber.QueueArn, subscriptionInfo.Topic.TopicArn);
            subscriptionInfo.CancelToken = queuePoller.Start(subscriptionInfo, messageHandler);
            return subscriptionInfo;
        }

        ~SubscriptionService()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Dispose implementation, ensures that all temporary listener queues are deleted.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposeManaged)
        {
            if (!isDisposed)
            {
                isDisposed = true;
                foreach(var subscription in subscriptions.Values)
                {
                    if (subscription.CancelToken!=null)
                    {
                        subscription.CancelToken.Cancel(true);
                    }
                    if (subscription.IsSubscribed)
                    {
                        amazonSnsFacade.UnsubscribeQueueFromTopic(subscription.SubscriptionArn);
                    }
                    subscriberRespoitroy.ExpireSubscriber(subscription.Subscriber);
                }
                if (disposeManaged)
                {
                    //dispose managed

                }
            }

        }
    }
}