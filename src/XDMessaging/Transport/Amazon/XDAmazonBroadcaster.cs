using Conditions;
using XDMessaging.Entities.Amazon;
using XDMessaging.Messages;
using XDMessaging.Serialization;
using XDMessaging.Transport.Amazon.Interfaces;
using XDMessaging.Transport.Amazon.Repositories;

namespace XDMessaging.Transport.Amazon
{
    // ReSharper disable once InconsistentNaming
    internal sealed class XDAmazonBroadcaster : IXDBroadcaster
    {
        private readonly IPublisherService publisherService;
        private readonly ISerializer serializer;
        private readonly TopicRepository topicRepository;

        internal XDAmazonBroadcaster(ISerializer serializer, IPublisherService publisherService,
            TopicRepository topicRepository)
        {
            serializer.Requires("serializer").IsNotNull();
            publisherService.Requires("publisherService").IsNotNull();
            topicRepository.Requires("topicRepository").IsNotNull();

            this.serializer = serializer;
            this.publisherService = publisherService;
            this.topicRepository = topicRepository;
        }

        public void SendToChannel(string channel, string message)
        {
            channel.Requires("channel").IsNotNull();
            message.Requires("message").IsNotNullOrWhiteSpace();

            SendToChannel(channel, typeof (string).AssemblyQualifiedName, message);
        }

        public void SendToChannel(string channel, object message)
        {
            channel.Requires("channel").IsNotNull();
            message.Requires("message").IsNotNull();

            var msg = serializer.Serialize(message);
            SendToChannel(channel, message.GetType().AssemblyQualifiedName, msg);
        }

        public bool IsAlive => AmazonAccountSettings.GetInstance().IsValid;

        private void SendToChannel(string channel, string dataType, string message)
        {
            channel.Requires("channel").IsNotNull();
            dataType.Requires("dataType").IsNotNull();
            message.Requires("message").IsNotNullOrWhiteSpace();

            var topic = topicRepository.GetTopic(channel);
            var dataGram = new DataGram(channel, dataType, message);
            var data = serializer.Serialize(dataGram);

            publisherService.Publish(topic, dataGram.Channel, data);
        }
    }
}