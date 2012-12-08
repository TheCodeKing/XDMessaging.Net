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
    internal sealed class TopicRepository : ITopicRepository
    {
        #region Constants and Fields

        private readonly IDictionary<string, Topic> topics =
            new Dictionary<string, Topic>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Regex channelValidatorRegex = new Regex("[^0-9a-zA-Z-_]", RegexOptions.Compiled);
        private readonly AmazonAccountSettings amazonAccountSettings;
        private readonly IAmazonSnsFacade amazonSnsFacade;

        #endregion

        #region Constructors and Destructors

        public TopicRepository(AmazonAccountSettings amazonAccountSettings, IAmazonSnsFacade amazonSnsFacade)
        {
            Validate.That(amazonAccountSettings).IsNotNull();
            Validate.That(amazonSnsFacade).IsNotNull();

            this.amazonAccountSettings = amazonAccountSettings;
            this.amazonSnsFacade = amazonSnsFacade;
        }

        #endregion

        #region Public Methods

        public Topic GetTopic(string channelName)
        {
            Validate.That(channelName).IsNotNullOrEmpty();

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

        private string GetTopicName(string channelName)
        {
            Validate.That(channelName).IsNotNullOrEmpty();

            var topicName = String.Concat(amazonAccountSettings.UniqueAppKey, "-", channelName);
            if (DoesNameRequireEscape(topicName))
            {
                var toEncodeAsBytes = Encoding.UTF8.GetBytes(topicName);
                topicName = Convert.ToBase64String(toEncodeAsBytes).Replace("+", "_").Replace("/", "-").TrimEnd('=');
            }
            if (topicName.Length > 80)
            {
                throw new ArgumentException(
                    String.Concat(
                        "The channelName/uniqueAppKey is too long and cannot be supported due to the AWS queue name length restrictions. Resultant name would be ",
                        topicName),
                    "channelName");
            }
            return topicName.ToLowerInvariant();
        }

        #endregion

        #region Methods

        private static bool DoesNameRequireEscape(string rawTopic)
        {
            return channelValidatorRegex.IsMatch(rawTopic);
        }

        #endregion
    }
}