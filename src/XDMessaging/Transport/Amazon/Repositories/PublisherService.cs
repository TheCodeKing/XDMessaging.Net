using Conditions;
using XDMessaging.Entities.Amazon;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Repositories
{
    internal sealed class PublisherService : IPublisherService
    {
        private readonly IAmazonSnsFacade amazonSnsFacade;

        public PublisherService(IAmazonSnsFacade amazonSnsFacade)
        {
            amazonSnsFacade.Requires().IsNotNull();

            this.amazonSnsFacade = amazonSnsFacade;
        }

        public void Publish(Topic topic, string subject, string message)
        {
            topic.Requires().IsNotNull();
            subject.Requires().IsNotNullOrWhiteSpace();
            message.Requires().IsNotNullOrWhiteSpace();

            amazonSnsFacade.PublishMessageToTopic(topic.TopicArn, subject, message);
        }
    }
}