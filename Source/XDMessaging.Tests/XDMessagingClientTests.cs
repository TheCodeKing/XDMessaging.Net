using NUnit.Framework;

namespace XDMessaging.Tests
{
    [TestFixture]
    public class XDMessagingClientTests
    {
        #region Public Methods

        [Test]
        public void GivenCompatibilityListenerImplThenShouldResolveSuccess()
        {
            // arrange
            var client = new XDMessagingClient();

            // act
            var instance = client.Listeners.GetListenerForMode(XDTransportMode.Compatibility);

            // assert
            Assert.That(instance, Is.Not.Null);
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
        public void GivenHighPerformanceUiModeListenerImplThenShouldResolveInstanceSuccess()
        {
            // arrange
            var client = new XDMessagingClient();

            // act
            var instance = client.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);

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

        #endregion
    }
}