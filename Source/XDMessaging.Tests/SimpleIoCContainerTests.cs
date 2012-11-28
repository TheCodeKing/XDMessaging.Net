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
using System.Collections.Specialized;
using NUnit.Framework;
using TheCodeKing.Utils.IoC;
using TheCodeKing.Utils.Serialization;
using XDMessaging.IoC;
using XDMessaging.Transport.Amazon;

namespace XDMessaging.Tests
{
    [TestFixture]
    public class SimpleIocContainerTests
    {
        #region Constants and Fields

        private IocContainer instance;

        #endregion

        #region Public Methods

        [Test]
        public void GivenAssembliesWithXDBroadcastImplementationAssertResolveSuccess()
        {
            var broadcast = instance.Resolve<IXDBroadcaster>(Convert.ToString(XDTransportMode.Compatibility));

            Assert.That(broadcast, Is.Not.Null);
        }

        [Test]
        public void GivenAssembliesWithXDListenerImplementationAssertResolveSuccess()
        {
            var broadcast = instance.Resolve<IXDListener>(Convert.ToString(XDTransportMode.Compatibility));

            Assert.That(broadcast, Is.Not.Null);
        }

        [Test]
        public void GivenAmazonFacadeImplementationAssertResolveSuccess()
        {
           var amazonFacade = instance.Resolve<IAmazonFacade>();
            Assert.That(amazonFacade, Is.Not.Null);
        }
        

        [SetUp]
        public void SetUp()
        {
            instance = SimpleIocContainerBootstrapper.GetInstance();
        }

        #endregion
    }
}