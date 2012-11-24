using System;
using Amazon.SQS.Model;

namespace XDMessaging.Transport.Amazon
{
    public interface IAwsQueueReceiver
    {
        void Start(SubscriptionInfo subscriptionInfo, Action<Message> onMessageReceived);

        void Stop(SubscriptionInfo subscriptionInfo);
    }
}