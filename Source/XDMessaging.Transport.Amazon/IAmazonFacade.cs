using System;
using System.Collections.Generic;
using Amazon.SQS.Model;

namespace XDMessaging.Transport.Amazon
{
    public interface IAmazonFacade
    {
        Uri CreateOrRetrieveQueue(string name, out string queueArn);
        Uri CreateOrRetrieveQueue(string name);
        string DeleteQueue(Uri queueUri);
        string CreateOrRetrieveTopic(string name);
        string SetSqsPolicyForSnsPublish(Uri queueUrl, string queueArn, string mytopicArn);
        string SubscribeQueueToTopic(string queueArn, string topicArn);
        string UnsubscribeQueueToTopic(string subscriptionArn);
        string PublishMessageToTopic(string topicArn, string subject, string message);
        IEnumerable<Message> ReadQueue(Uri queueUri);
        string DeleteMessage(Uri queueUrl, string messageId);
    }
}