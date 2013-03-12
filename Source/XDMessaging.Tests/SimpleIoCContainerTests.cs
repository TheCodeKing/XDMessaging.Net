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
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using NUnit.Framework;
using TheCodeKing.Utils.IoC;
using XDMessaging.IdentityProviders;
using XDMessaging.IoC;
using XDMessaging.Transport.Amazon.Interfaces;

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
        public void GivenIocContainerInitializeShouldCompleteInLessThanHalfSecond()
        {
            // arrange
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // act
            SimpleIocContainerBootstrapper.GetInstance(true);
            stopwatch.Stop();

            // assert
            Assert.That(stopwatch.Elapsed.Milliseconds, Is.LessThanOrEqualTo(500));
        }


        [Test]
        public void GivenLifetimeSingletonThenReturnSameInstanceSuccess()
        {
            instance.UpdateRegistration<IIdentityProvider, MachineNameIdentityProvider>(LifeTime.Singleton);
            var identityProvider1 = instance.Resolve<IIdentityProvider>();
            var identityProvider2 = instance.Resolve<IIdentityProvider>();

            Assert.That(identityProvider1, Is.SameAs(identityProvider2));
        }

        [Test]
        public void GivenLifetimeInstanceThenReturnSameInstanceSuccess()
        {
            instance.UpdateRegistration<IIdentityProvider, MachineNameIdentityProvider>(LifeTime.Instance);
            var identityProvider1 = instance.Resolve<IIdentityProvider>();
            var identityProvider2 = instance.Resolve<IIdentityProvider>();

            Assert.That(identityProvider1, Is.Not.SameAs(identityProvider2));
        }

        [Test]
        public void GivenCloneContainerResolveSuccess()
        {
            var copyContainer = instance.Clone();
            var identityProvider = copyContainer.Resolve<IIdentityProvider>();

            Assert.That(identityProvider, Is.Not.Null);
            Assert.That(identityProvider.GetType(), Is.EqualTo(typeof(UniqueIdentityProvider)));
        }

        [Test]
        public void GivenCloneContainerAndUpdateRegistrationDoesNotAffectOriginalResolveSuccess()
        {
            var copyContainer = instance.Use<IIdentityProvider, MachineNameIdentityProvider>();
            var identityProvider = instance.Resolve<IIdentityProvider>();
            var copyIdentityProvider = copyContainer.Resolve<IIdentityProvider>();

            Assert.That(identityProvider, Is.Not.Null);
            Assert.That(identityProvider.GetType(), Is.EqualTo(typeof(UniqueIdentityProvider)));
            Assert.That(copyIdentityProvider.GetType(), Is.EqualTo(typeof(MachineNameIdentityProvider)));

        }

        [Test]
        public void GivenContainerAndUpdateRegistrationResolveSuccess()
        {
            var identityProvider =
                instance.Use<IIdentityProvider, MachineNameIdentityProvider>().Resolve<IIdentityProvider>();

            Assert.That(identityProvider, Is.Not.Null);
            Assert.That(identityProvider.GetType(), Is.EqualTo(typeof(MachineNameIdentityProvider)));
        }

        [Test]
        public void GivenCopyContainerAndUpdateRegistrationResolveSuccess()
        {
            var identityProvider = instance.Use<IIdentityProvider, MachineNameIdentityProvider>().Resolve<IIdentityProvider>();
            var originalIdentityProvider = instance.Resolve<IIdentityProvider>();
            
            Assert.That(identityProvider, Is.Not.Null);
            Assert.That(identityProvider.GetType(), Is.EqualTo(typeof(MachineNameIdentityProvider)));
            Assert.That(originalIdentityProvider.GetType(), Is.EqualTo(typeof(UniqueIdentityProvider)));
        }


        [Test]
        public void GivenAmazonFacadeImplementationAssertResolveSuccess()
        {
            var amazonFacade = instance.Resolve<IAmazonSnsFacade>();
            Assert.That(amazonFacade, Is.Not.Null);
        }

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
        public void GivenIocSingletonRegistrationWhenResolveTypeSameInstanceSuccess()
        {
            instance.Register(() => new NameValueCollection(0), LifeTime.Singleton);

            var value1 = instance.Resolve<NameValueCollection>();
            var value2 = instance.Resolve<NameValueCollection>();

            Assert.That(value1, Is.SameAs(value2));
        }

        [Test]
        public void GivenIocInstanceRegistrationWhenResolveTypeDifferentInstanceSuccess()
        {
            instance.Register(() => new NameValueCollection(0), LifeTime.Instance);

            var value1 = instance.Resolve<NameValueCollection>();
            var value2 = instance.Resolve<NameValueCollection>();

            Assert.That(value1, Is.Not.SameAs(value2));
        }

        [SetUp]
        public void SetUp()
        {
            instance = SimpleIocContainerBootstrapper.GetInstance(true);
        }

        #endregion
    }
}