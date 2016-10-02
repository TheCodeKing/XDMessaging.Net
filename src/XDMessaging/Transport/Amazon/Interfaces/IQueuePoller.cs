using System;
using System.Threading;
using Amazon.SQS.Model;
using XDMessaging.Entities.Amazon;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface IQueuePoller
    {
        CancellationTokenSource Start(SubscriptionInfo subscriptionInfo, Action<Message> onMessageReceived);
    }
}