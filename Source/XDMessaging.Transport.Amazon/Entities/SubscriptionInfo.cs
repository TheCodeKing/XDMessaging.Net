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

namespace XDMessaging.Transport.Amazon.Entities
{
    internal sealed class SubscriptionInfo
    {
        #region Properties

        public string ChannelName { get; set; }

        public bool IsSubscribed
        {
            get { return !String.IsNullOrEmpty(SubscriptionArn); }
        }

        public CancellationTokenSource CancelToken;
        public Topic Topic { set; get; }
        public Subscriber Subscriber { set; get; }
        public string SubscriptionArn { set; get; }

        #endregion
    }
}