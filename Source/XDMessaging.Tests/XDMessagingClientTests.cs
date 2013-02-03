using NUnit.Framework;
using TheCodeKing.Utils.IoC;
using XDMessaging.Transport.Amazon.Entities;

namespace XDMessaging.Tests
{
    [TestFixture]
    public class XDMessagingClientTests
    {
        [Test]
        public void GivenAmazonSettingsThenShouldResolveSuccess()
        {
            // arrange
            var client = new XDMessagingClient().WithAmazonSettings("xx", "xx");

            // act
            var accessKey = AmazonAccountSettings.GetInstance().AccessKey;
            var accessKey2 = client.Broadcasters.Container.Resolve<AmazonAccountSettings>().AccessKey;


            // assert
            Assert.That(accessKey, Is.EqualTo("xx"));
            Assert.That(accessKey2, Is.EqualTo("xx"));
        }

        [Test]
        public void GivenCompatibilityModeThenShouldResolveSuccess()
        {
            // arrange
            var client = new XDMessagingClient();

            // act
            var instance = client.Broadcasters.GetBroadcasterForMode(XDTransportMode.Compatibility);

            // assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void GivenHighPerformanceUiModeBroadcastImplThenShouldResolveSuccess()
        {
            // arrange
            var client = new XDMessagingClient();

            // act
            var instance = client.Broadcasters.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);

            // assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void GivenRemoteNetworkModeImplThenShouldResolveInstanceSuccess()
        {
            // arrange
            var client = new XDMessagingClient();

            // act
            var instance = client.Listeners.GetListenerForMode(XDTransportMode.RemoteNetwork);

            // assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void GivenRemoteNetworkModeImplThenShouldResolveSuccess()
        {
            // arrange
            var client = new XDMessagingClient();

            // act
            var instance = client.Broadcasters.GetBroadcasterForMode(XDTransportMode.RemoteNetwork);

            // assert
            Assert.That(instance, Is.Not.Null);
        }
    }
}