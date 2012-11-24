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

namespace XDMessaging.Transport.Amazon
{
    public interface IAwsQueueReceiver
    {
        #region Public Methods

        void Start(SubscriptionInfo subscriptionInfo, Action<Message> onMessageReceived);

        void Stop(SubscriptionInfo subscriptionInfo);

        #endregion
    }
}