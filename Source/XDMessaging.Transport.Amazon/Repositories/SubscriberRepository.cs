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
using System.Text;
using System.Text.RegularExpressions;
using TheCodeKing.Utils.Contract;
using XDMessaging.Transport.Amazon.Entities;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Repositories
{
    internal sealed class SubscriberRepository : ISubscriberRepository
    {
        #region Constants and Fields

        private readonly IDictionary<string, Subscriber> subscribers = new Dictionary<string, Subscriber>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Regex channelValidatorRegex = new Regex("^[^0-9a-zA-Z-_]*$", RegexOptions.Compiled);
        private readonly AmazonAccountSettings amazonAccountSettings;
        private readonly IAmazonSqsFacade amazonSqsFacade;

        #endregion

        #region Constructors and Destructors

        public SubscriberRepository(AmazonAccountSettings amazonAccountSettings, IAmazonSqsFacade amazonSqsFacade)
        {
            Validate.That(amazonAccountSettings).IsNotNull();
            Validate.That(amazonSqsFacade).IsNotNull();

            this.amazonAccountSettings = amazonAccountSettings;
            this.amazonSqsFacade = amazonSqsFacade;
        }

        #endregion

        #region Implemented Interfaces

        #region ISubscriberRepository

        public Subscriber GetSubscriber(string channelName, string subscriberId)
        {
            Validate.That(channelName).IsNotNullOrEmpty();
            Validate.That(subscriberId).IsNotNullOrEmpty();

            var uniqueQueue = GetQueueName(channelName, subscriberId);
            if (subscribers.ContainsKey(uniqueQueue))
            {
                return subscribers[uniqueQueue];
            }
            var queueUrl = amazonSqsFacade.CreateOrRetrieveQueue(uniqueQueue);
            var queueArn = amazonSqsFacade.GetQueueArn(queueUrl);

            var subscriber = new Subscriber(uniqueQueue, queueUrl, queueArn);
            subscribers[uniqueQueue] = subscriber;
            return subscriber;
        }

        public void ExpireSubscriber(Subscriber subscriber)
        {
            if (subscribers.ContainsKey(subscriber.Name))
            {
                subscribers.Remove(subscriber.Name);
                amazonSqsFacade.DeleteQueue(subscriber.QueueUrl);
            }

        }

        #endregion

        #endregion

        #region Methods

        private static bool DoesNameRequireEscape(string rawTopic)
        {
            return channelValidatorRegex.IsMatch(rawTopic);
        }

        private string GetQueueName(string channelName, string subscriber)
        {
            var uniqueQueue = String.Concat(amazonAccountSettings.UniqueAppKey, "-", subscriber, "-", channelName);
            if (DoesNameRequireEscape(uniqueQueue))
            {
                var encodeAsBytes = Encoding.UTF8.GetBytes(amazonAccountSettings.UniqueAppKey + uniqueQueue);
                uniqueQueue = Convert.ToBase64String(encodeAsBytes).Replace("+", "_").Replace("/", "-").TrimEnd('=');
            }
            if (uniqueQueue.Length > 80)
            {
                throw new ArgumentException(
                    String.Concat(
                        "The channelName/subscriber/uniqueAppKey is too long and cannot be supported due to the AWS queue name length restrictions. Resultant name would be ",
                        uniqueQueue),
                    "channelName");
            }
            return uniqueQueue.ToLowerInvariant();
        }

        #endregion
    }
}