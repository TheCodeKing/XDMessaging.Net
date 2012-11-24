using System;

namespace XDMessaging.Transport.Amazon
{
    public sealed class SubscriptionInfo
    {
        #region Properties

        public string ChannelName { get; set; }

        public bool IsSubscribed
        {
            get { return !String.IsNullOrEmpty(SubscriptionArn); }
        }

        public string QueueArn { set; get; }
        public Uri QueueUrl { set; get; }
        public string SubscriptionArn { set; get; }
        public string TopicArn { set; get; }
        public string TopicName { set; get; }

        #endregion
    }
}