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
using System.Text;
using System.Text.RegularExpressions;
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Transport.Amazon
{
    internal static class NameHelper
    {
        #region Constants and Fields

        public static readonly Regex ChannelValidatorRegex = new Regex("^[^0-9a-zA-Z-_]*$", RegexOptions.Compiled);

        #endregion

        #region Public Methods

        public static string GetQueueNameFromListenerId(string uniqueAppKey, string channelName, string listenerId)
        {
            Validate.That(uniqueAppKey).IsNotNullOrEmpty();
            Validate.That(channelName).IsNotNullOrEmpty();
            Validate.That(listenerId).IsNotNullOrEmpty();

            var uniqueQueue = string.Concat(uniqueAppKey, "-", listenerId, "-", channelName);
            if (NameRequiresEscape(uniqueQueue))
            {
                var toEncodeAsBytes = Encoding.UTF8.GetBytes(uniqueAppKey + uniqueQueue);
                uniqueQueue = Convert.ToBase64String(toEncodeAsBytes).Replace("+", "_").Replace("/", "-").TrimEnd('=');
            }
            if (uniqueQueue.Length > 80)
            {
                throw new ArgumentException(
                    string.Concat(
                        "The channelName/uniqueAppKey is too long and cannot be supported due to the AWS queue name length restrictions. Resultant name would be ",
                        uniqueQueue),
                    "channelName");
            }
            return uniqueQueue;
        }

        public static string GetTopicNameFromChannel(string uniqueAppKey, string channelName)
        {
            Validate.That(uniqueAppKey).IsNotNullOrEmpty();
            Validate.That(channelName).IsNotNullOrEmpty();

            var topicName = string.Concat(uniqueAppKey, "-", channelName);
            if (NameRequiresEscape(topicName))
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
            return topicName;
        }

        #endregion

        #region Methods

        private static bool NameRequiresEscape(string rawTopic)
        {
            return ChannelValidatorRegex.IsMatch(rawTopic);
        }

        #endregion
    }
}