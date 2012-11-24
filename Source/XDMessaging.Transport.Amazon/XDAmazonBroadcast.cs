using System;
using System.Collections.Concurrent;
using System.Configuration;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;
using TheCodeKing.Utils.IoC;
using XDMessaging.Core;
using XDMessaging.Core.IoC;
using XDMessaging.Core.Message;
using XDMessaging.Core.Specialized;

namespace XDMessaging.Transport.Amazon
{
    [TransportModeHint(XDTransportMode.RemoteNetwork)]
    public sealed class XDAmazonBroadcast : IXDBroadcast
    {
        #region Constants and Fields

        private readonly ConcurrentDictionary<string, string> registeredTopics =
            new ConcurrentDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IAmazonFacade amazonFacade;
        private readonly AmazonAccountSettings amazonAccountSettings;
        private readonly ISerializer serializer;

        #endregion

        #region Constructors and Destructors

        static XDAmazonBroadcast()
        {
            var instance = SimpleIoCContainerBootstrapper.GetInstance();
            instance.Register<ISerializer, SpecializedSerializer>();
            instance.Register(() => ConfigurationManager.AppSettings);
            instance.Register(AmazonAccountSettings.GetInstance);
            instance.Register<IAmazonFacade, AmazonFacade>();
        }

        public XDAmazonBroadcast(ISerializer serializer, IAmazonFacade amazonFacade, AmazonAccountSettings amazonAccountSettings)
        {
            Validate.That(serializer).IsNotNull();
            Validate.That(amazonFacade).IsNotNull();
            Validate.That(amazonAccountSettings).IsNotNull();

            this.serializer = serializer;
            this.amazonFacade = amazonFacade;
            this.amazonAccountSettings = amazonAccountSettings;
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcast

        public void SendToChannel(string channel, string message)
        {
            Validate.That(channel).IsNotNull();
            Validate.That(message).IsNotNullOrEmpty();

            var topicArn = registeredTopics.GetOrAdd(channel, CreateChannel);
            var dataGram = new DataGram(channel, message);
            var data = serializer.Serialize(dataGram);
            amazonFacade.PublishMessageToTopic(topicArn, channel, data);
        }

        public string CreateChannel(string channelName)
        {
            var topicName = NameHelper.GetTopicNameFromChannel(amazonAccountSettings.UniqueAppKey, channelName);
            return amazonFacade.CreateOrRetrieveTopic(topicName);
        }

        public void SendToChannel(string channel, object message)
        {
            Validate.That(channel).IsNotNull();
            Validate.That(message).IsNotNull();

            var msg = serializer.Serialize(message);
            SendToChannel(channel, msg);

        }

        #endregion

        #endregion
    }
}