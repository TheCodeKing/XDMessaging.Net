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
using Amazon.SQS.Model;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Entities;
using XDMessaging.IdentityProviders;
using XDMessaging.Messages;
using XDMessaging.Transport.Amazon.Entities;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon
{
    [XDListenerHint(XDTransportMode.RemoteNetwork)]
// ReSharper disable InconsistentNaming
    internal sealed class XDAmazonListener : IXDListener
// ReSharper restore InconsistentNaming
    {
        #region Constants and Fields

        private readonly ISerializer serializer;
        private readonly ISubscriberRepository subscriberRepository;
        private readonly ISubscriptionService subscriptionService;
        private readonly ITopicRepository topicRepository;
        private readonly string uniqueInstanceId = Guid.NewGuid().ToString("N");
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        internal XDAmazonListener(IIdentityProvider identityProvider, ISerializer serializer, ITopicRepository topicRepository,
                                  ISubscriberRepository subscriberRepository, ISubscriptionService subscriptionService)
        {
            Validate.That(identityProvider).IsNotNull();
            Validate.That(serializer).IsNotNull();
            Validate.That(topicRepository).IsNotNull();
            Validate.That(subscriberRepository).IsNotNull();
            Validate.That(subscriptionService).IsNotNull();

            this.uniqueInstanceId = identityProvider.GetUniqueId();
            this.serializer = serializer;
            this.topicRepository = topicRepository;
            this.subscriberRepository = subscriberRepository;
            this.subscriptionService = subscriptionService;
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

        public event Listeners.XDMessageHandler MessageReceived;

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

            var topic = topicRepository.GetTopic(channelName);
            var subscriber = subscriberRepository.GetSubscriber(channelName, uniqueInstanceId);

            subscriptionService.Subscribe(topic, subscriber, OnMessageReceived);
        }


        public void UnRegisterChannel(string channelName)
        {
            Validate.That(channelName).IsNotNull();

            if (disposed)
            {
                return;
            }

            var topic = topicRepository.GetTopic(channelName);
            var subscriber = subscriberRepository.GetSubscriber(channelName, uniqueInstanceId);

            subscriptionService.Unsubscribe(topic, subscriber);
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
                        var del = MessageReceived.GetInvocationList();
                        foreach (Listeners.XDMessageHandler msg in del)
                        {
                            MessageReceived -= msg;
                        }
                    }
                }
                subscriptionService.Dispose();
            }
        }

        private void OnMessageReceived(Message message)
        {
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

        #endregion
    }
}