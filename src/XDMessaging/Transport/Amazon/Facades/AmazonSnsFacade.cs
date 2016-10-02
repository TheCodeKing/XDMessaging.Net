using System;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Conditions;
using XDMessaging.Entities.Amazon;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Facades
{
    internal sealed class AmazonSnsFacade : IAmazonSnsFacade
    {
        private const string SqsProtocol = "sqs";
        private readonly Func<IAmazonSimpleNotificationService> amazonSnsFactory;

        public AmazonSnsFacade(AmazonAccountSettings amazonAccountSettings)
        {
            amazonAccountSettings.Requires("amazonAccountSettings").IsNotNull();

            amazonSnsFactory = () =>
                AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(amazonAccountSettings.AccessKey,
                    amazonAccountSettings.SecretKey,
                    amazonAccountSettings.RegionEndPoint.
                        ToRegionEndpoint());
        }

        public string CreateOrRetrieveTopic(string name)
        {
            name.Requires("name").IsNotNullOrWhiteSpace();

            var topicRequest = new CreateTopicRequest(name);
            using (var sns = amazonSnsFactory())
            {
                var topicResponse = sns.CreateTopic(topicRequest);
                return topicResponse?.TopicArn;
            }
        }

        public string PublishMessageToTopic(string topicArn, string subject, string message)
        {
            topicArn.Requires("topicArn").IsNotNullOrWhiteSpace();
            subject.Requires("subject").IsNotNullOrWhiteSpace();
            message.Requires("message").IsNotNullOrWhiteSpace();

            var publishRequest = new PublishRequest(topicArn, message, subject);
            using (var sns = amazonSnsFactory())
            {
                var result = sns.Publish(publishRequest);
                return result?.MessageId;
            }
        }

        public string SubscribeQueueToTopic(string queueArn, string topicArn)
        {
            queueArn.Requires("queueArn").IsNotNullOrWhiteSpace();
            topicArn.Requires("topicArn").IsNotNullOrWhiteSpace();

            var subScribeRequest = new SubscribeRequest(topicArn, SqsProtocol, queueArn);
            using (var sns = amazonSnsFactory())
            {
                var response = sns.Subscribe(subScribeRequest);
                return response?.SubscriptionArn;
            }
        }

        public string UnsubscribeQueueFromTopic(string subscriptionArn)
        {
            subscriptionArn.Requires("subscriptionArn").IsNotNullOrWhiteSpace();

            var unsubscribeRequest = new UnsubscribeRequest(subscriptionArn);
            using (var sns = amazonSnsFactory())
            {
                var response = sns.Unsubscribe(unsubscribeRequest);
                return response.ResponseMetadata.RequestId;
            }
        }
    }
}