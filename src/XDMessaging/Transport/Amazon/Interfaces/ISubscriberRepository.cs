using XDMessaging.Entities.Amazon;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface ISubscriberRepository
    {
        void ExpireSubscriber(Subscriber subscriber);
        Subscriber GetSubscriber(string channelName, string subscriberId, bool longLived);

        Subscriber GetSubscriber(string channelName, string subscriber);
    }
}