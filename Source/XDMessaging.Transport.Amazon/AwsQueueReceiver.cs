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
using System.Threading;
using Amazon.SQS.Model;
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Transport.Amazon
{
    internal class AwsQueueReceiver : IAwsQueueReceiver
    {
        #region Constants and Fields

        private readonly IAmazonFacade amazonFacade;
        private readonly ConcurrentDictionary<string, ReadTask> tracking = new ConcurrentDictionary<string, ReadTask>();

        #endregion

        #region Constructors and Destructors

        public AwsQueueReceiver(IAmazonFacade amazonFacade)
        {
            Validate.That(amazonFacade).IsNotNull();

            this.amazonFacade = amazonFacade;
        }

        #endregion

        #region Implemented Interfaces

        #region IAwsQueueReceiver

        public void Start(SubscriptionInfo subscriptionInfo, Action<Message> onMessageReceived)
        {
            Validate.That(subscriptionInfo).IsNotNull();
            Validate.That(onMessageReceived).IsNotNull();

            var readTask = new ReadTask {SubscriptionInfo = subscriptionInfo, OnMessageReceived = onMessageReceived};
            tracking.AddOrUpdate(subscriptionInfo.ChannelName, k => readTask, (k, g) =>
                                                                                  {
                                                                                      g.IsCancelled = true;
                                                                                      return readTask;
                                                                                  });
            ThreadPool.QueueUserWorkItem(AsyncReadQueueLoop, readTask);
        }

        public void Stop(SubscriptionInfo subscriptionInfo)
        {
            Validate.That(subscriptionInfo).IsNotNull();

            ReadTask readTask;
            tracking.TryRemove(subscriptionInfo.ChannelName, out readTask);
            if (readTask != null)
            {
                readTask.IsCancelled = true;
            }
        }

        #endregion

        #endregion

        #region Methods

        private void AsyncReadQueueLoop(object state)
        {
            var readTask = (ReadTask) state;

            while (!readTask.IsCancelled && readTask.SubscriptionInfo.IsSubscribed)
            {
                foreach (var message in amazonFacade.ReadQueue(readTask.SubscriptionInfo.QueueUrl))
                {
                    amazonFacade.DeleteMessage(readTask.SubscriptionInfo.QueueUrl, message.ReceiptHandle);
                    readTask.OnMessageReceived(message);
                }
            }
        }

        #endregion

        internal class ReadTask
        {
            #region Properties

            public bool IsCancelled { get; set; }
            public Action<Message> OnMessageReceived { get; set; }
            public SubscriptionInfo SubscriptionInfo { get; set; }

            #endregion
        }
    }
}