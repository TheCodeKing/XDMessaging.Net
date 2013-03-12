/*=============================================================================
*
*	(C) Copyright 2013, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
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
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using TheCodeKing.Utils.Contract;
using XDMessaging.Transport.Amazon.Entities;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon
{
    internal sealed class QueuePoller : IQueuePoller
    {
        #region Constants and Fields

        private readonly IAmazonSqsFacade amazonFacade;

        #endregion

        #region Constructors and Destructors

        public QueuePoller(IAmazonSqsFacade amazonFacade)
        {
            Validate.That(amazonFacade).IsNotNull();

            this.amazonFacade = amazonFacade;
        }

        #endregion

        #region Implemented Interfaces

        #region IQueuePoller

        public CancellationTokenSource Start(SubscriptionInfo subscriptionInfo, Action<Message> messageHandler)
        {
            Validate.That(subscriptionInfo).IsNotNull();

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
                                      }, source.Token);
            return source;
        }

        #endregion

        #endregion
    }
}