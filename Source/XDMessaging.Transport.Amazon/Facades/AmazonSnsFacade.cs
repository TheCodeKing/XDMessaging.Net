/*=============================================================================
*
*	(C) Copyright 2013, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
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
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using TheCodeKing.Utils.Contract;
using XDMessaging.Transport.Amazon.Entities;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Facades
{
    internal sealed class AmazonSnsFacade : IAmazonSnsFacade
    {
        #region Constants and Fields

        private const string sqsProtocol = "sqs";
        private readonly Func<AmazonSimpleNotificationService> amazonSnsFactory;

        #endregion

        #region Constructors and Destructors

        public AmazonSnsFacade(AmazonAccountSettings amazonAccountSettings)
        {
            Validate.That(amazonAccountSettings).IsNotNull();

            amazonSnsFactory = () =>
                               AWSClientFactory.CreateAmazonSNSClient(amazonAccountSettings.AccessKey,
                                                                      amazonAccountSettings.SecretKey,
                                                                      amazonAccountSettings.RegionEndPoint.
                                                                          ToRegionEndpoint());
        }

        #endregion

        #region Implemented Interfaces

        #region IAmazonSnsFacade

        public string CreateOrRetrieveTopic(string name)
        {
            Validate.That(name).IsNotNullOrEmpty();

            var topicRequest = new CreateTopicRequest().WithName(name);
            using (var sns = amazonSnsFactory())
            {
                var topicResponse = sns.CreateTopic(topicRequest);
                return topicResponse.CreateTopicResult.TopicArn;
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
            using (var sns = amazonSnsFactory())
            {
                var result = sns.Publish(publishRequest);
                return result.PublishResult.MessageId;
            }
        }

        public string SubscribeQueueToTopic(string queueArn, string topicArn)
        {
            Validate.That(queueArn).IsNotNullOrEmpty();
            Validate.That(topicArn).IsNotNullOrEmpty();

            var subScribeRequest =
                new SubscribeRequest().WithEndpoint(queueArn).WithProtocol(sqsProtocol).WithTopicArn(topicArn);
            using (var sns = amazonSnsFactory())
            {
                var response = sns.Subscribe(subScribeRequest);
                return response.SubscribeResult.SubscriptionArn;
            }
        }

        public string UnsubscribeQueueFromTopic(string subscriptionArn)
        {
            Validate.That(subscriptionArn).IsNotNullOrEmpty();

            var unsubscribeRequest = new UnsubscribeRequest().WithSubscriptionArn(subscriptionArn);
            using (var sns = amazonSnsFactory())
            {
                var response = sns.Unsubscribe(unsubscribeRequest);
                return response.ResponseMetadata.RequestId;
            }
        }

        #endregion

        #endregion
    }
}