/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using System;
using System.Collections.Concurrent;
using System.Configuration;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Messages;
using XDMessaging.Specialized;

namespace XDMessaging.Transport.Amazon
{
    [XDBroadcasterHint(XDTransportMode.RemoteNetwork)]
    public sealed class XDAmazonBroadcaster : IXDBroadcaster
    {
        #region Constants and Fields

        private readonly IAmazonFacade amazonFacade;

        private readonly ConcurrentDictionary<string, string> registeredTopics =
            new ConcurrentDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        private readonly ISerializer serializer;

        #endregion

        #region Constructors and Destructors

        public XDAmazonBroadcaster(ISerializer serializer, IAmazonFacade amazonFacade)
        {
            Validate.That(serializer).IsNotNull();
            Validate.That(amazonFacade).IsNotNull();

            this.serializer = serializer;
            this.amazonFacade = amazonFacade;
        }

        #endregion

        #region Public Methods

        public string CreateChannel(string channelName)
        {
            Validate.That(channelName).IsNotNullOrEmpty();

            var topicName = amazonFacade.GetTopicNameFromChannel(channelName);
            return amazonFacade.CreateOrRetrieveTopic(topicName);
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcaster

        public void SendToChannel(string channel, string message)
        {
            Validate.That(channel).IsNotNull();
            Validate.That(message).IsNotNullOrEmpty();

            var topicArn = registeredTopics.GetOrAdd(channel, CreateChannel);
            var dataGram = new DataGram(channel, message);
            var data = serializer.Serialize(dataGram);

            Func<string, string, string, string> action = amazonFacade.PublishMessageToTopic;
            action.BeginInvoke(topicArn, channel, data, (r) => action.EndInvoke(r), null);
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

        #region Methods

        /// <summary>
        ///   Initialize method called from XDMessaging.Core before the instance is constructed.
        ///   This allows external classes to registered dependencies with the IocContainer.
        /// </summary>
        /// <param name = "container">The IocContainer instance used to construct this class.</param>
        private static void Initialize(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            container.Scan.ScanEmbeddedResources(typeof (XDAmazonBroadcaster).Assembly);
            container.Register<ISerializer, SpecializedSerializer>();
            container.Register(() => ConfigurationManager.AppSettings);
            container.Register(AmazonAccountSettings.GetInstance);
            container.Register<IAmazonFacade, AmazonFacade>();
        }

        #endregion
    }
}