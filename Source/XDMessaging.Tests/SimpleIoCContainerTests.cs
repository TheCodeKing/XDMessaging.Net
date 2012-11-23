using System;
using NUnit.Framework;
using TheCodeKing.Utils.IoC;
using XDMessaging.Core;
using XDMessaging.Core.IoC;

namespace XDMessaging.Tests
{
    [TestFixture]
    public class SimpleIoCContainerTests
    {
        #region Constants and Fields

        private IoCContainer instance;

        #endregion

        #region Public Methods

        [Test]
        public void GivenAssembliesWithXDBroadcastImplementationAssertResolveSuccess()
        {
            var broadcast = instance.Resolve<IXDBroadcast>(Convert.ToString(XDTransportMode.Compatibility));

            Assert.That(broadcast, Is.Not.Null);
        }

        [Test]
        public void GivenAssembliesWithXDListenerImplementationAssertResolveSuccess()
        {
            var broadcast = instance.Resolve<IXDListener>(Convert.ToString(XDTransportMode.Compatibility));

            Assert.That(broadcast, Is.Not.Null);
        }

        [SetUp]
        public void SetUp()
        {
            instance = SimpleIoCContainerBootstrapper.GetInstance();
        }

        #endregion
    }
}