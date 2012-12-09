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
using Amazon.SQS;
using Amazon.SQS.Model;
using TheCodeKing.Utils.Contract;
using XDMessaging.Transport.Amazon.Entities;
using XDMessaging.Transport.Amazon.Interfaces;
using Attribute = Amazon.SQS.Model.Attribute;

namespace XDMessaging.Transport.Amazon.Facades
{
    internal sealed class AmazonSqsFacade : IAmazonSqsFacade
    {
        #region Constants and Fields

        private const string conditionSourceArn = "aws:SourceArn";
        private const string queueArnAttributeName = "QueueArn";
        private const int receiveMessageWaitTimeSeconds = 20;
        private const int messageRetentionPeriodSeconds = 60;
        private const string receiveMessageWaitTimeSecondsAttributeName = "ReceiveMessageWaitTimeSeconds";
        private const string messageRetentionPeriodAttributeName = "MessageRetentionPeriod";
        private readonly Func<AmazonSQS> amazonSqsFactory;

        #endregion

        #region Constructors and Destructors

        public AmazonSqsFacade(AmazonAccountSettings amazonAccountSettings)
        {
            Validate.That(amazonAccountSettings).IsNotNull();

            amazonSqsFactory = () =>
                               AWSClientFactory.CreateAmazonSQSClient(amazonAccountSettings.AccessKey,
                                                                      amazonAccountSettings.SecretKey,
                                                                      amazonAccountSettings.RegionEndPoint.
                                                                          ToRegionEndpoint());
        }

        #endregion

        #region Implemented Interfaces

        #region IAmazonSqsFacade

        public Uri CreateOrRetrieveQueue(string name)
        {
            Validate.That(name).IsNotNullOrEmpty();

            var longPollAttribute = new Attribute().WithName(receiveMessageWaitTimeSecondsAttributeName)
                .WithValue(Convert.ToString(receiveMessageWaitTimeSeconds));
            var messageRetainAttribute = new Attribute().WithName(messageRetentionPeriodAttributeName)
                .WithValue(Convert.ToString(messageRetentionPeriodSeconds));
            var sqsRequest = new CreateQueueRequest().WithQueueName(name).WithAttribute(longPollAttribute, messageRetainAttribute);
            using (var sqs = amazonSqsFactory())
            {
                var createQueueResponse = sqs.CreateQueue(sqsRequest);
                return new Uri(createQueueResponse.CreateQueueResult.QueueUrl);
            }
        }

        public string DeleteMessage(Uri queueUrl, string receiptHandle)
        {
            Validate.That(queueUrl).IsNotNull();
            Validate.That(receiptHandle).IsNotNullOrEmpty();

            var deleteMessageRequest =
                new DeleteMessageRequest().WithQueueUrl(queueUrl.AbsoluteUri).WithReceiptHandle(receiptHandle);
            using (var sqs = amazonSqsFactory())
            {
                var deleteResponseMessage = sqs.DeleteMessage(deleteMessageRequest);
                return deleteResponseMessage.ResponseMetadata.RequestId;
            }
        }

        public string DeleteQueue(Uri queueUri)
        {
            Validate.That(queueUri).IsNotNull();

            var sqsDeleteRequest = new DeleteQueueRequest().WithQueueUrl(queueUri.AbsoluteUri);
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
            // get queueArn
            var attributeRequest =
                new GetQueueAttributesRequest().WithQueueUrl(queueUrl.AbsoluteUri).WithAttributeName(
                    queueArnAttributeName);
            using (var sqs = amazonSqsFactory())
            {
                var queueAttributes = sqs.GetQueueAttributes(attributeRequest);
                return queueAttributes.GetQueueAttributesResult.Attribute[0].Value;
            }
        }

        public IEnumerable<Message> ReadQueue(Uri queueUrl)
        {
            Validate.That(queueUrl).IsNotNull();

            var receiveMessageRequest = new ReceiveMessageRequest().WithQueueUrl(queueUrl.AbsoluteUri);
            using (var sqs = amazonSqsFactory())
            {
                var response = sqs.ReceiveMessage(receiveMessageRequest);
                return response.ReceiveMessageResult.Message;
            }
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
                                                                  conditionSourceArn, mytopicArn)));

            var setQueueAttributesRequest =
                new SetQueueAttributesRequest().WithQueueUrl(queueUrl.AbsoluteUri).WithPolicy(sqsPolicy.ToJson());
            using (var sqs = amazonSqsFactory())
            {
                var response = sqs.SetQueueAttributes(setQueueAttributesRequest);
                return response.ResponseMetadata.RequestId;
            }
        }

        #endregion

        #endregion
    }
}