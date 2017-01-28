using NUnit.Framework;

namespace XDMessaging.Test
{
    [TestFixture]
    public class XDMessagingClientTests
    {
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