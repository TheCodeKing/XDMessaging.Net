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
using TheCodeKing.Utils.IoC;
using XDMessaging.Transport.Amazon.Entities;
using XDMessaging.Transport.Amazon.Facades;
using XDMessaging.Transport.Amazon.Interfaces;
using XDMessaging.Transport.Amazon.Repositories;

namespace XDMessaging.Transport.Amazon
{
    [IocInitialize]
    public static class IocConfiguration
    {
        #region Methods

        /// <summary>
        ///   Initialize method called from XDMessaging.Core before the instance is constructed.
        ///   This allows external classes to registered dependencies with the IocContainer.
        /// </summary>
        /// <param name = "container">The IocContainer instance used to construct this class.</param>
        private static void Initialize(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            container.Register(AmazonAccountSettings.GetInstance, LifeTime.Singleton);
            container.Register<ITopicRepository, TopicRepository>(LifeTime.Singleton);
            container.Register<ISubscriberRepository, SubscriberRepository>(LifeTime.Instance);
            container.Register<IQueuePoller, QueuePoller>(LifeTime.Singleton);
            container.Register<IAmazonSqsFacade, AmazonSqsFacade>(LifeTime.Singleton);
            container.Register<IAmazonSnsFacade, AmazonSnsFacade>(LifeTime.Singleton);
        }

        #endregion
    }
}