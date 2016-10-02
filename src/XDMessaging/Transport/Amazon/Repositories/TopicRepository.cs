using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Conditions;
using XDMessaging.Entities.Amazon;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Repositories
{
    internal sealed class TopicRepository : ITopicRepository
    {
        private static readonly Regex ChannelValidatorRegex = new Regex("[^0-9a-zA-Z-_]", RegexOptions.Compiled);
        private readonly AmazonAccountSettings amazonAccountSettings;
        private readonly IAmazonSnsFacade amazonSnsFacade;

        private readonly IDictionary<string, Topic> topics =
            new Dictionary<string, Topic>(StringComparer.InvariantCultureIgnoreCase);

        public TopicRepository(AmazonAccountSettings amazonAccountSettings, IAmazonSnsFacade amazonSnsFacade)
        {
            amazonAccountSettings.Requires("amazonAccountSettings").IsNotNull();
            amazonSnsFacade.Requires("amazonSnsFacade").IsNotNull();

            this.amazonAccountSettings = amazonAccountSettings;
            this.amazonSnsFacade = amazonSnsFacade;
        }

        public Topic GetTopic(string channelName)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();

            var topicName = GetTopicName(channelName);
            var topicArn = amazonSnsFacade.CreateOrRetrieveTopic(topicName);
            if (topics.ContainsKey(channelName))
            {
                return topics[channelName];
            }
            var topic = new Topic(topicName, topicArn);
            topics[channelName] = topic;
            return topic;
        }

        private static bool DoesNameRequireEscape(string rawTopic)
        {
            return ChannelValidatorRegex.IsMatch(rawTopic);
        }

        private string GetTopicName(string channelName)
        {
            channelName.Requires("channelName").IsNotNullOrWhiteSpace();

            var topicName = string.Concat(amazonAccountSettings.UniqueAppKey, "-", channelName);
            if (DoesNameRequireEscape(topicName))
            {
                var toEncodeAsBytes = Encoding.UTF8.GetBytes(topicName);
                topicName = Convert.ToBase64String(toEncodeAsBytes).Replace("+", "_").Replace("/", "-").TrimEnd('=');
            }
            if (topicName.Length > 80)
            {
                throw new ArgumentException(
                    string.Concat(
                        "The channelName/uniqueAppKey is too long and cannot be supported due to the AWS queue name length restrictions. Resultant name would be ",
                        topicName),
                    "channelName");
            }
            return topicName.ToLowerInvariant();
        }
    }
}