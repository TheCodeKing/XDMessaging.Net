using System;
using System.Collections.Generic;
using Amazon;
using Amazon.Auth.AccessControlPolicy;
using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using Amazon.SQS;
using Amazon.SQS.Model;
using Conditions;
using XDMessaging.Entities.Amazon;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Facades
{
    internal sealed class AmazonSqsFacade : IAmazonSqsFacade
    {
        private const string ConditionSourceArn = "aws:SourceArn";
        private const int MessageRetentionPeriodSeconds = 60;
        private const int ReceiveMessageWaitTimeSeconds = 20;
        private readonly Func<IAmazonSQS> amazonSqsFactory;

        public AmazonSqsFacade(AmazonAccountSettings amazonAccountSettings)
        {
            amazonAccountSettings.Requires("amazonAccountSettings").IsNotNull();

            amazonSqsFactory = () =>
                AWSClientFactory.CreateAmazonSQSClient(amazonAccountSettings.AccessKey,
                    amazonAccountSettings.SecretKey,
                    amazonAccountSettings.RegionEndPoint.
                        ToRegionEndpoint());
        }

        public Uri CreateOrRetrieveQueue(string name)
        {
            name.Requires("name").IsNotNullOrWhiteSpace();

            var attributes = new Dictionary<string, string>
            {
                {QueueAttributeName.ReceiveMessageWaitTimeSeconds, Convert.ToString(ReceiveMessageWaitTimeSeconds)},
                {QueueAttributeName.MessageRetentionPeriod, Convert.ToString(MessageRetentionPeriodSeconds)}
            };

            var sqsRequest = new CreateQueueRequest(name) {Attributes = attributes};
            using (var sqs = amazonSqsFactory())
            {
                var createQueueResponse = sqs.CreateQueue(sqsRequest);
                return new Uri(createQueueResponse.QueueUrl);
            }
        }

        public string DeleteMessage(Uri queueUrl, string receiptHandle)
        {
            queueUrl.Requires("queueUrl").IsNotNull();
            receiptHandle.Requires("receiptHandle").IsNotNullOrWhiteSpace();

            var deleteMessageRequest = new DeleteMessageRequest(queueUrl.AbsoluteUri, receiptHandle);
            using (var sqs = amazonSqsFactory())
            {
                var deleteResponseMessage = sqs.DeleteMessage(deleteMessageRequest);
                return deleteResponseMessage.ResponseMetadata.RequestId;
            }
        }

        public string DeleteQueue(Uri queueUri)
        {
            queueUri.Requires("queueUri").IsNotNull();

            var sqsDeleteRequest = new DeleteQueueRequest(queueUri.AbsoluteUri);
            try
            {
                using (var sqs = amazonSqsFactory())
                {
                    var deleteQueueResponse = sqs.DeleteQueue(sqsDeleteRequest);
                    return deleteQueueResponse.ResponseMetadata.RequestId;
                }
            }
            catch (AmazonSQSException)
            {
                return null;
            }
        }

        public string GetQueueArn(Uri queueUrl)
        {
            queueUrl.Requires("queueUrl").IsNotNull();

            var attributes = new List<string> {QueueAttributeName.QueueArn};
            var attributeRequest = new GetQueueAttributesRequest(queueUrl.AbsoluteUri, attributes);
            using (var sqs = amazonSqsFactory())
            {
                var queueAttributes = sqs.GetQueueAttributes(attributeRequest);
                return queueAttributes.Attributes[QueueAttributeName.QueueArn];
            }
        }

        public IEnumerable<Message> ReadQueue(Uri queueUrl)
        {
            queueUrl.Requires("queueUrl").IsNotNull();

            var receiveMessageRequest = new ReceiveMessageRequest(queueUrl.AbsoluteUri);
            using (var sqs = amazonSqsFactory())
            {
                var response = sqs.ReceiveMessage(receiveMessageRequest);
                return response.Messages;
            }
        }

        public string SetSqsPolicyForSnsPublish(Uri queueUrl, string queueArn, string mytopicArn)
        {
            queueUrl.Requires("queueUrl").IsNotNull();
            queueArn.Requires("queueArn").IsNotNullOrWhiteSpace();
            mytopicArn.Requires("mytopicArn").IsNotNullOrWhiteSpace();

            var sqsPolicy = new Policy().WithStatements(
                new Statement(Statement.StatementEffect.Allow)
                    .WithResources(new Resource(queueArn))
                    .WithPrincipals(Principal.AllUsers)
                    .WithActionIdentifiers(SQSActionIdentifiers.SendMessage)
                    .WithConditions(ConditionFactory.NewCondition(ConditionFactory.ArnComparisonType.ArnEquals,
                        ConditionSourceArn, mytopicArn)));

            var attributes = new Dictionary<string, string>
            {
                {QueueAttributeName.Policy, sqsPolicy.ToJson()}
            };

            var setQueueAttributesRequest = new SetQueueAttributesRequest(queueUrl.AbsoluteUri, attributes);
            using (var sqs = amazonSqsFactory())
            {
                var response = sqs.SetQueueAttributes(setQueueAttributesRequest);
                return response.ResponseMetadata.RequestId;
            }
        }
    }
}