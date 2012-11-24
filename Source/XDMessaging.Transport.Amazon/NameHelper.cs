using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDMessaging.Transport.Amazon
{
    internal static class NameHelper
    {
        public static string GetTopicNameFromChannel(string uniqueAppKey, string channelName)
        {
            var toEncodeAsBytes = Encoding.UTF8.GetBytes(uniqueAppKey + channelName);
            var queueName = Convert.ToBase64String(toEncodeAsBytes).Replace("+", "_").Replace("/", "-").TrimEnd('=');
            if (queueName.Length > 80)
            {
                throw new ArgumentException(
                    "The encoded resourceUrl is too long and cannot be supported, to AWS queue name length restrictions.",
                    "resourceUrl");
            }
            return queueName;
        }

        public static string GetQueueNameFromChannel(string uniqueAppKey, string channelName)
        {
            var uniqueQueue = Guid.NewGuid().ToString("N");
            var toEncodeAsBytes = Encoding.UTF8.GetBytes(uniqueAppKey + uniqueQueue);
            var queueName = Convert.ToBase64String(toEncodeAsBytes).Replace("+", "_").Replace("/", "-").TrimEnd('=');
            if (queueName.Length > 80)
            {
                throw new ArgumentException(
                    "The encoded resourceUrl is too long and cannot be supported, to AWS queue name length restrictions.",
                    "resourceUrl");
            }
            return queueName;
        }
    }
}
