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
using System.Collections.Generic;
using Amazon.SQS.Model;

namespace XDMessaging.Transport.Amazon
{
    public interface IAmazonFacade
    {
        #region Public Methods

        Uri CreateOrRetrieveQueue(string name, out string queueArn);
        Uri CreateOrRetrieveQueue(string name);
        string CreateOrRetrieveTopic(string name);
        string DeleteMessage(Uri queueUrl, string receiptHandle);
        string DeleteQueue(Uri queueUri);
        string PublishMessageToTopic(string topicArn, string subject, string message);
        IEnumerable<Message> ReadQueue(Uri queueUri);
        string SetSqsPolicyForSnsPublish(Uri queueUrl, string queueArn, string mytopicArn);
        string SubscribeQueueToTopic(string queueArn, string topicArn);
        string UnsubscribeQueueToTopic(string subscriptionArn);

        string GetQueueNameFromListenerId(string channelName, string listenerId);
        string GetTopicNameFromChannel(string channelName);

        #endregion
    }
}