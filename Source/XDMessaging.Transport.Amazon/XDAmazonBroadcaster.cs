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
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Messages;
using XDMessaging.Transport.Amazon.Interfaces;
using XDMessaging.Transport.Amazon.Repositories;

namespace XDMessaging.Transport.Amazon
{
    [XDBroadcasterHint(XDTransportMode.RemoteNetwork)]
// ReSharper disable InconsistentNaming
    internal sealed class XDAmazonBroadcaster : IXDBroadcaster
// ReSharper restore InconsistentNaming
    {
        #region Constants and Fields

        private readonly ISerializer serializer;
        private readonly ISubscriptionService subscriptionService;
        private readonly TopicRepository topicRepository;

        #endregion

        #region Constructors and Destructors

        internal XDAmazonBroadcaster(ISerializer serializer, ISubscriptionService subscriptionService,
                                     TopicRepository topicRepository)
        {
            Validate.That(serializer).IsNotNull();
            Validate.That(subscriptionService).IsNotNull();
            Validate.That(topicRepository).IsNotNull();

            this.serializer = serializer;
            this.subscriptionService = subscriptionService;
            this.topicRepository = topicRepository;
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcaster

        public void SendToChannel(string channel, string message)
        {
            Validate.That(channel).IsNotNull();
            Validate.That(message).IsNotNullOrEmpty();

            var topic = topicRepository.GetTopic(channel);
            var dataGram = new DataGram(channel, message);
            var data = serializer.Serialize(dataGram);

            subscriptionService.Publish(topic, dataGram.Channel, data);
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