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

namespace XDMessaging.Transport.Amazon
{
    public sealed class SubscriptionInfo
    {
        #region Properties

        public string ChannelName { get; set; }

        public bool IsSubscribed
        {
            get { return !String.IsNullOrEmpty(SubscriptionArn); }
        }

        public string QueueArn { set; get; }
        public Uri QueueUrl { set; get; }
        public string SubscriptionArn { set; get; }
        public string TopicArn { set; get; }
        public string TopicName { set; get; }

        #endregion
    }
}