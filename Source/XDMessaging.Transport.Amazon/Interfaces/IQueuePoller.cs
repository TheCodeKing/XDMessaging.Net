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
using Amazon.SQS.Model;
using XDMessaging.Transport.Amazon.Entities;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface IQueuePoller
    {
        #region Public Methods

        CancellationTokenSource Start(SubscriptionInfo subscriptionInfo, Action<Message> onMessageReceived);

        #endregion
    }
}