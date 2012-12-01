using System;
using Amazon.SQS.Model;
using XDMessaging.Transport.Amazon.Entities;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface ISubscriptionService : IDisposable
    {
        #region Public Methods

        SubscriptionInfo Subscribe(Topic topic, Subscriber subscriber, Action<Message> onMessageReceived);

        SubscriptionInfo Unsubscribe(Topic topic, Subscriber subscriber);

        bool IsSubscribed(Topic topic, Subscriber subscriber);

        #endregion

        void Publish(Topic topic, string subject, string message);
    }
}