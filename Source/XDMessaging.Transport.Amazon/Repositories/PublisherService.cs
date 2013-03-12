/*=============================================================================
*
*	(C) Copyright 2013, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
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
using XDMessaging.Transport.Amazon.Entities;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon.Repositories
{
    internal sealed class PublisherService : IPublisherService
    {
        #region Constants and Fields

        private readonly IAmazonSnsFacade amazonSnsFacade;

        #endregion

        #region Constructors and Destructors

        public PublisherService(IAmazonSnsFacade amazonSnsFacade)
        {
            Validate.That(amazonSnsFacade).IsNotNull();

            this.amazonSnsFacade = amazonSnsFacade;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Publish messsage to topic.
        /// </summary>
        /// <param name = "topic">The topic to publish the message to. The topic represents a channel.</param>
        /// <param name = "subject">The suject used when publishing to the topic.</param>
        /// <param name = "message">The body message used when publishing to the topic.</param>
        public void Publish(Topic topic, string subject, string message)
        {
            Validate.That(topic).IsNotNull();
            Validate.That(subject).IsNotNullOrEmpty();
            Validate.That(message).IsNotNullOrEmpty();

            amazonSnsFacade.PublishMessageToTopic(topic.TopicArn, subject, message);
        }

        #endregion
    }
}