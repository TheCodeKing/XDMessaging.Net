using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Amazon.ElasticMapReduce.Model;
using Amazon.SQS.Model;
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Transport.Amazon
{
    internal class AwsQueueReceiver : IAwsQueueReceiver
    {
        internal class ReadTask
        {
            public SubscriptionInfo SubscriptionInfo { get; set; }
            public Action<Message> OnMessageReceived { get; set; }
            public bool IsCancelled { get; set; }
        }

        private readonly IAmazonFacade amazonFacade;
        private readonly ConcurrentDictionary<string, ReadTask> tracking = new ConcurrentDictionary<string, ReadTask>();

        public AwsQueueReceiver(IAmazonFacade amazonFacade)
        {
            Validate.That(amazonFacade).IsNotNull();

            this.amazonFacade = amazonFacade;
        }

        public void Start(SubscriptionInfo subscriptionInfo, Action<Message> onMessageReceived)
        {
            var readTask = new ReadTask { SubscriptionInfo = subscriptionInfo, OnMessageReceived = onMessageReceived };
            tracking.AddOrUpdate(subscriptionInfo.ChannelName, k => readTask, (k, g) =>
                                                                                  {
                                                                                      g.IsCancelled = true;
                                                                                      return readTask;
                                                                                  });
            ThreadPool.QueueUserWorkItem(AsyncReadQueueLoop, readTask);
        }

        public void Stop(SubscriptionInfo subscriptionInfo)
        {
            ReadTask readTask;
            tracking.TryRemove(subscriptionInfo.ChannelName, out readTask);
            if (readTask != null)
            {
                readTask.IsCancelled = true;
            }
        }

        private void AsyncReadQueueLoop(object state)
        {
            var readTask = (ReadTask)state;

            while (readTask.IsCancelled || readTask.SubscriptionInfo.IsSubscribed)
            {
                foreach (var message in amazonFacade.ReadQueue(readTask.SubscriptionInfo.QueueUrl))
                {
                    if (readTask.IsCancelled || !readTask.SubscriptionInfo.IsSubscribed)
                    {
                        break;
                    }
                    amazonFacade.DeleteMessage(readTask.SubscriptionInfo.QueueUrl, message.ReceiptHandle);
                    readTask.OnMessageReceived(message);
                }
            }
        }
    }
}