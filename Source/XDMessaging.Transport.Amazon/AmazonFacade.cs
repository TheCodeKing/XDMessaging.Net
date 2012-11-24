using System;
using System.Collections.Generic;
using Amazon;
using Amazon.Auth.AccessControlPolicy;
using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using TheCodeKing.Utils.Contract;
using XDMessaging.Core;
using Attribute = Amazon.SQS.Model.Attribute;

namespace XDMessaging.Transport.Amazon
{
    internal sealed class AmazonFacade : IAmazonFacade
    {
        private readonly AmazonAccountSettings amazonAccountSettings;
        private RegionEndpoint regionEndpoint = null;

        #region Constants and Fields

        private AmazonSimpleNotificationService sns;
        private AmazonSQS sqs;

        #endregion

        #region Constructors and Destructors

        private AmazonSimpleNotificationService Sns
        {
            get
            {
                if (regionEndpoint != amazonAccountSettings.RegionEndPoint)
                {
                    sns = null;
                    sqs = null;
                    regionEndpoint = amazonAccountSettings.RegionEndPoint;
                }
                if (sns != null)
                {
                    return sns;
                }
                sns = amazonAccountSettings.RegionEndPoint == null 
                    ? AWSClientFactory.CreateAmazonSNSClient(amazonAccountSettings.AccessKey, amazonAccountSettings.SecretKey) 
                    : AWSClientFactory.CreateAmazonSNSClient(amazonAccountSettings.AccessKey, amazonAccountSettings.SecretKey, amazonAccountSettings.RegionEndPoint);
                regionEndpoint = amazonAccountSettings.RegionEndPoint;
                return sns;
            }
        }

        private AmazonSQS Sqs
        {
            get
            {
                if (regionEndpoint != amazonAccountSettings.RegionEndPoint)
                {
                    sns = null;
                    sqs = null;
                    regionEndpoint = amazonAccountSettings.RegionEndPoint;
                }
                if (sqs!=null)
                {
                    return sqs;
                }
                sqs = amazonAccountSettings.RegionEndPoint == null 
                                      ? AWSClientFactory.CreateAmazonSQSClient(amazonAccountSettings.AccessKey, amazonAccountSettings.SecretKey) 
                                      : AWSClientFactory.CreateAmazonSQSClient(amazonAccountSettings.AccessKey, amazonAccountSettings.SecretKey, amazonAccountSettings.RegionEndPoint);
                regionEndpoint = amazonAccountSettings.RegionEndPoint;
                return sqs;
            }
        }


        public AmazonFacade(AmazonAccountSettings amazonAccountSettings)
        {
            Validate.That(amazonAccountSettings).IsNotNull();

            this.amazonAccountSettings = amazonAccountSettings;
        }

        #endregion

        #region Public Methods

        public Uri CreateOrRetrieveQueue(string name, out string queueArn)
        {
            Validate.That(name).IsNotNullOrEmpty();

            var queueUrl = CreateOrRetrieveQueue(name);
            queueArn = GetQueueArn(queueUrl);
            return queueUrl;
        }

        public Uri CreateOrRetrieveQueue(string name)
        {
            Validate.That(name).IsNotNullOrEmpty();

            var sqsRequest = new CreateQueueRequest {QueueName = name};
            var createQueueResponse = Sqs.CreateQueue(sqsRequest);
            return new Uri(createQueueResponse.CreateQueueResult.QueueUrl);
        }

        public string CreateOrRetrieveTopic(string name)
        {
            Validate.That(name).IsNotNullOrEmpty();

            var topicRequest = new CreateTopicRequest {Name = name};
            var topicResponse = Sns.CreateTopic(topicRequest);
            return topicResponse.CreateTopicResult.TopicArn;
        }

        public void SetSqsPolicyForSnsPublish(Uri queueUrl, string mytopicArn)
        {
            Validate.That(queueUrl).IsNotNull();
            Validate.That(mytopicArn).IsNotNullOrEmpty();

            var sqsPolicy = new Policy().WithStatements(
                new Statement(Statement.StatementEffect.Allow)
                    .WithPrincipals(Principal.AllUsers)
                    .WithActionIdentifiers(SQSActionIdentifiers.SendMessage)
                    .WithConditions(ConditionFactory.NewSourceArnCondition(mytopicArn)));

            var policyAttribute = new Attribute {Name = "Policy", Value = sqsPolicy.ToJson()};
            var setQueueAttributesRequest = new SetQueueAttributesRequest
                                                {
                                                    Attribute = new List<Attribute> {policyAttribute},
                                                    QueueUrl = queueUrl.AbsoluteUri
                                                };
            Sqs.SetQueueAttributes(setQueueAttributesRequest);
        }

        public string SubscribeQueueToTopic(string queueArn, string topicArn)
        {
            Validate.That(queueArn).IsNotNullOrEmpty();
            Validate.That(topicArn).IsNotNullOrEmpty();

            var subScribeRequest = new SubscribeRequest {Endpoint = queueArn, Protocol = "sqs", TopicArn = topicArn};
            var response = Sns.Subscribe(subScribeRequest);
            return response.SubscribeResult.SubscriptionArn;
        }

        #endregion

        #region Methods

        private static List<T> CreateList<T>(params T[] items)
        {
            return new List<T>(items);
        }

        private string GetQueueArn(Uri queueUrl)
        {
            // get queueArn
            var attributeRequest = new GetQueueAttributesRequest
                                       {
                                           AttributeName = CreateList("QueueArn"),
                                           QueueUrl = queueUrl.AbsoluteUri
                                       };
            var queueAttributes = Sqs.GetQueueAttributes(attributeRequest);
            return queueAttributes.GetQueueAttributesResult.Attribute[0].Value;
        }

        #endregion
    }
}