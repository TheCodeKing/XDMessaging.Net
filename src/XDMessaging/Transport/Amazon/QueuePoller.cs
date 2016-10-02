using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Conditions;
using XDMessaging.Entities.Amazon;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon
{
    internal sealed class QueuePoller : IQueuePoller
    {
        private readonly IAmazonSqsFacade amazonFacade;

        public QueuePoller(IAmazonSqsFacade amazonFacade)
        {
            amazonFacade.Requires("amazonFacade").IsNotNull();

            this.amazonFacade = amazonFacade;
        }

        public CancellationTokenSource Start(SubscriptionInfo subscriptionInfo, Action<Message> messageHandler)
        {
            subscriptionInfo.Requires("subscriptionInfo").IsNotNull();

            var source = new CancellationTokenSource();
            Task.Factory.StartNew(o =>
            {
                while (!source.Token.IsCancellationRequested)
                {
                    foreach (var message in amazonFacade.ReadQueue(subscriptionInfo.Subscriber.QueueUrl))
                    {
                        if (messageHandler != null && subscriptionInfo.IsSubscribed)
                        {
                            messageHandler(message);
                        }
                        amazonFacade.DeleteMessage(subscriptionInfo.Subscriber.QueueUrl,
                            message.ReceiptHandle);
                    }
                }
            }, source.Token, source.Token);
            return source;
        }
    }
}