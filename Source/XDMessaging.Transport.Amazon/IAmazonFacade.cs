using System;

namespace XDMessaging.Transport.Amazon
{
    public interface IAmazonFacade
    {
        Uri CreateOrRetrieveQueue(string name, out string queueArn);
        Uri CreateOrRetrieveQueue(string name);
        string CreateOrRetrieveTopic(string name);
        void SetSqsPolicyForSnsPublish(Uri queueUrl, string mytopicArn);
        string SubscribeQueueToTopic(string queueArn, string topicArn);
    }
}