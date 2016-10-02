using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Conditions;
using XDMessaging.Entities.Amazon;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Repositories
{
    internal sealed class SubscriberRepository : ISubscriberRepository
    {
        private static readonly Regex ChannelValidatorRegex = new Regex("[^0-9a-zA-Z-_]", RegexOptions.Compiled);
        private readonly AmazonAccountSettings amazonAccountSettings;
        private readonly IAmazonSqsFacade amazonSqsFacade;

        private readonly IDictionary<string, Subscriber> subscribers =
            new Dictionary<string, Subscriber>(StringComparer.InvariantCultureIgnoreCase);

        public SubscriberRepository(AmazonAccountSettings amazonAccountSettings, IAmazonSqsFacade amazonSqsFacade)
        {
            amazonAccountSettings.Requires("amazonAccountSettings").IsNotNull();
            amazonSqsFacade.Requires("amazonSqsFacade").IsNotNull();

            this.amazonAccountSettings = amazonAccountSettings;
            this.amazonSqsFacade = amazonSqsFacade;
        }

        public void ExpireSubscriber(Subscriber subscriber)
        {
            subscriber.Requires("subscriber").IsNotNull();

            if (!subscribers.ContainsKey(subscriber.Name))
            {
                return;
            }

            subscribers.Remove(subscriber.Name);
            if (!subscriber.LongLived)
            {
                amazonSqsFacade.DeleteQueue(subscriber.QueueUrl);
            }
        }

        public Subscriber GetSubscriber(string channelName, string subscriberId)
        {
            return GetSubscriber(channelName, subscriberId, false);
        }

        public Subscriber GetSubscriber(string channelName, string subscriberId, bool longLived)
        {
            channelName.Requires().IsNotNullOrWhiteSpace();
            subscriberId.Requires().IsNotNullOrWhiteSpace();

            var uniqueQueue = GetQueueName(channelName, subscriberId);
            if (subscribers.ContainsKey(uniqueQueue))
            {
                return subscribers[uniqueQueue];
            }

            var queueUrl = amazonSqsFacade.CreateOrRetrieveQueue(uniqueQueue);
            var queueArn = amazonSqsFacade.GetQueueArn(queueUrl);

            var subscriber = new Subscriber(uniqueQueue, queueUrl, queueArn, longLived);
            subscribers[uniqueQueue] = subscriber;
            return subscriber;
        }

        private static bool DoesNameRequireEscape(string rawTopic)
        {
            return ChannelValidatorRegex.IsMatch(rawTopic);
        }

        public string CalculateMd5Hash(string input)
        {
            input.Requires("input").IsNotNullOrWhiteSpace();

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private string GetQueueName(string channelName, string subscriber)
        {
            var uniqueQueue = string.Concat(amazonAccountSettings.UniqueAppKey, "-", subscriber, "-", channelName);
            if (DoesNameRequireEscape(uniqueQueue))
            {
                uniqueQueue = CalculateMd5Hash(uniqueQueue);
            }

            if (uniqueQueue.Length > 80)
            {
                throw new ArgumentException(
                    string.Concat(
                        "The channelName/subscriber/uniqueAppKey is too long and cannot be supported due to the AWS queue name length restrictions. Resultant name would be ",
                        uniqueQueue),
                    "channelName");
            }

            return uniqueQueue.ToLowerInvariant();
        }
    }
}