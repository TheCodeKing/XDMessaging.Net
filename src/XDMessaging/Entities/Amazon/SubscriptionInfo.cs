using System.Threading;

namespace XDMessaging.Entities.Amazon
{
    internal sealed class SubscriptionInfo
    {
        public CancellationTokenSource CancelToken;

        public string ChannelName { get; set; }

        public bool IsSubscribed => !string.IsNullOrEmpty(SubscriptionArn);

        public Subscriber Subscriber { set; get; }

        public string SubscriptionArn { set; get; }

        public Topic Topic { set; get; }
    }
}