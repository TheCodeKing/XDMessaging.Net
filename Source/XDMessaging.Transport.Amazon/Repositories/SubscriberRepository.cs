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
using System.Security.Cryptography;
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
        private static readonly Regex channelValidatorRegex = new Regex("[^0-9a-zA-Z-_]", RegexOptions.Compiled);
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
                uniqueQueue = CalculateMd5Hash(uniqueQueue);
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

        public string CalculateMd5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        #endregion
    }
}