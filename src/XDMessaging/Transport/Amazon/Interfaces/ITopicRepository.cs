using XDMessaging.Entities.Amazon;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface ITopicRepository
    {
        Topic GetTopic(string channelName);
    }
}