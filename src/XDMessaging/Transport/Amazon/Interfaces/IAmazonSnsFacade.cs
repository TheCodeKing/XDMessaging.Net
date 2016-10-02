namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface IAmazonSnsFacade
    {
        string CreateOrRetrieveTopic(string name);
        string PublishMessageToTopic(string topicArn, string subject, string message);
        string SubscribeQueueToTopic(string queueArn, string topicArn);
        string UnsubscribeQueueFromTopic(string subscriptionArn);
    }
}