using System;
using System.Collections.Generic;
using Amazon.SQS.Model;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface IAmazonSqsFacade
    {
        Uri CreateOrRetrieveQueue(string name);
        string DeleteMessage(Uri queueUrl, string receiptHandle);
        string DeleteQueue(Uri queueUri);
        string GetQueueArn(Uri queueUrl);
        IEnumerable<Message> ReadQueue(Uri queueUri);
        string SetSqsPolicyForSnsPublish(Uri queueUrl, string queueArn, string mytopicArn);
    }
}