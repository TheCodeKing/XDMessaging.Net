using XDMessaging.Transport.Amazon.Entities;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface ITopicRepository
    {
        Topic GetTopic(string channelName);
    }
}