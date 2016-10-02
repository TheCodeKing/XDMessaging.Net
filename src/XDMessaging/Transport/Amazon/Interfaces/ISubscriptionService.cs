using System;
using Amazon.SQS.Model;
using XDMessaging.Entities.Amazon;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface ISubscriptionService : IDisposable
    {
        bool IsSubscribed(Topic topic, Subscriber subscriber);
        SubscriptionInfo Subscribe(Topic topic, Subscriber subscriber, Action<Message> onMessageReceived);

        SubscriptionInfo Unsubscribe(Topic topic, Subscriber subscriber);
    }
}