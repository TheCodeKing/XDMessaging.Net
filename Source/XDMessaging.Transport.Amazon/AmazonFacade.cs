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
using Amazon;
using Amazon.Auth.AccessControlPolicy;
using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using TheCodeKing.Utils.Contract;
using Attribute = Amazon.SQS.Model.Attribute;

namespace XDMessaging.Transport.Amazon
{
    internal sealed class AmazonFacade : IAmazonFacade
    {
        #region Constants and Fields

        private readonly AmazonAccountSettings amazonAccountSettings;
        private RegionEndpoint regionEndpoint;

        private AmazonSimpleNotificationService sns;
        private AmazonSQS sqs;

        #endregion

        #region Constructors and Destructors

        public AmazonFacade(AmazonAccountSettings amazonAccountSettings)
        {
            Validate.That(amazonAccountSettings).IsNotNull();

            this.amazonAccountSettings = amazonAccountSettings;
        }

        #endregion

        #region Properties

        private AmazonSimpleNotificationService Sns
        {
            get
            {
                if (regionEndpoint != (RegionEndpoint)amazonAccountSettings.RegionEndPoint)
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
                          ? AWSClientFactory.CreateAmazonSNSClient(amazonAccountSettings.AccessKey,
                                                                   amazonAccountSettings.SecretKey)
                          : AWSClientFactory.CreateAmazonSNSClient(amazonAccountSettings.AccessKey,
                                                                   amazonAccountSettings.SecretKey,
                                                                   amazonAccountSettings.RegionEndPoint);
                regionEndpoint = amazonAccountSettings.RegionEndPoint;
                return sns;
            }
        }

        private AmazonSQS Sqs
        {
            get
            {
                if (regionEndpoint != (RegionEndpoint)amazonAccountSettings.RegionEndPoint)
                {
                    sns = null;
                    sqs = null;
                    regionEndpoint = amazonAccountSettings.RegionEndPoint;
                }
                if (sqs != null)
                {
                    return sqs;
                }
                sqs = amazonAccountSettings.RegionEndPoint == null
                          ? AWSClientFactory.CreateAmazonSQSClient(amazonAccountSettings.AccessKey,
                                                                   amazonAccountSettings.SecretKey)
                          : AWSClientFactory.CreateAmazonSQSClient(amazonAccountSettings.AccessKey,
                                                                   amazonAccountSettings.SecretKey,
                                                                   amazonAccountSettings.RegionEndPoint);
                regionEndpoint = amazonAccountSettings.RegionEndPoint;
                return sqs;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IAmazonFacade

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

            var longPollAttribute = new Attribute().WithName("ReceiveMessageWaitTimeSeconds").WithValue("20");
            var sqsRequest = new CreateQueueRequest().WithQueueName(name).WithAttribute(longPollAttribute);
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

        public string DeleteMessage(Uri queueUrl, string receiptHandle)
        {
            Validate.That(queueUrl).IsNotNull();
            Validate.That(receiptHandle).IsNotNullOrEmpty();

            var deleteMessageRequest =
                new DeleteMessageRequest().WithQueueUrl(queueUrl.AbsoluteUri).WithReceiptHandle(receiptHandle);
            var deleteResponseMessage = Sqs.DeleteMessage(deleteMessageRequest);
            return deleteResponseMessage.ResponseMetadata.RequestId;
        }

        public string DeleteQueue(Uri queueUri)
        {
            Validate.That(queueUri).IsNotNull();

            var sqsDeleteRequest = new DeleteQueueRequest().WithQueueUrl(queueUri.AbsoluteUri);
            try
            {
                var deleteQueueResponse = Sqs.DeleteQueue(sqsDeleteRequest);
                return deleteQueueResponse.ResponseMetadata.RequestId;
            }
            catch (AmazonSQSException)
            {
                return null;
            }
        }

        public string PublishMessageToTopic(string topicArn, string subject, string message)
        {
            Validate.That(topicArn).IsNotNullOrEmpty();
            Validate.That(subject).IsNotNullOrEmpty();
            Validate.That(message).IsNotNullOrEmpty();

            var publishRequest = new PublishRequest()
                .WithSubject(subject)
                .WithMessage(message)
                .WithTopicArn(topicArn);
            var result = Sns.Publish(publishRequest);
            return result.PublishResult.MessageId;
        }

        public IEnumerable<Message> ReadQueue(Uri queueUrl)
        {
            Validate.That(queueUrl).IsNotNull();

            var receiveMessageRequest = new ReceiveMessageRequest().WithQueueUrl(queueUrl.AbsoluteUri);
            var response = Sqs.ReceiveMessage(receiveMessageRequest);
            return response.ReceiveMessageResult.Message;
        }

        public string SetSqsPolicyForSnsPublish(Uri queueUrl, string queueArn, string mytopicArn)
        {
            Validate.That(queueUrl).IsNotNull();
            Validate.That(queueArn).IsNotNullOrEmpty();
            Validate.That(mytopicArn).IsNotNullOrEmpty();

            var sqsPolicy = new Policy().WithStatements(
                new Statement(Statement.StatementEffect.Allow)
                    .WithResources(new Resource(queueArn))
                    .WithPrincipals(Principal.AllUsers)
                    .WithActionIdentifiers(SQSActionIdentifiers.SendMessage)
                    .WithConditions(ConditionFactory.NewCondition(ConditionFactory.ArnComparisonType.ArnEquals,
                                                                  "aws:SourceArn", mytopicArn)));

            var setQueueAttributesRequest =
                new SetQueueAttributesRequest().WithQueueUrl(queueUrl.AbsoluteUri).WithPolicy(sqsPolicy.ToJson());
            var response = Sqs.SetQueueAttributes(setQueueAttributesRequest);
            return response.ResponseMetadata.RequestId;
        }

        public string SubscribeQueueToTopic(string queueArn, string topicArn)
        {
            Validate.That(queueArn).IsNotNullOrEmpty();
            Validate.That(topicArn).IsNotNullOrEmpty();

            var subScribeRequest =
                new SubscribeRequest().WithEndpoint(queueArn).WithProtocol("sqs").WithTopicArn(topicArn);
            var response = Sns.Subscribe(subScribeRequest);
            return response.SubscribeResult.SubscriptionArn;
        }

        public string UnsubscribeQueueToTopic(string subscriptionArn)
        {
            Validate.That(subscriptionArn).IsNotNullOrEmpty();

            var unsubscribeRequest = new UnsubscribeRequest().WithSubscriptionArn(subscriptionArn);
            var response = Sns.Unsubscribe(unsubscribeRequest);
            return response.ResponseMetadata.RequestId;
        }

        #endregion

        #endregion

        #region Methods

        private string GetQueueArn(Uri queueUrl)
        {
            // get queueArn
            var attributeRequest =
                new GetQueueAttributesRequest().WithQueueUrl(queueUrl.AbsoluteUri).WithAttributeName("QueueArn");
            var queueAttributes = Sqs.GetQueueAttributes(attributeRequest);
            return queueAttributes.GetQueueAttributesResult.Attribute[0].Value;
        }

        #endregion
    }
}