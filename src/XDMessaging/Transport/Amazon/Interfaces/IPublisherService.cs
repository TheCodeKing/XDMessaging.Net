using XDMessaging.Entities.Amazon;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface IPublisherService
    {
        void Publish(Topic topic, string subject, string message);
    }
}