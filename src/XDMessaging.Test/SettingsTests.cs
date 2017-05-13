using NUnit.Framework;
using XDMessaging.Config;

namespace XDMessaging.Test
{
    [TestFixture]
    public class SettingsTests
    {
        [Test]
        public void GivenSettingsImplThenShouldResolveAppSettingsIoStreamMessageTimeoutInMillisecondsValueSuccess()
        {
            // arrange

            // act
            var messageTimeoutInMilliseconds = Settings.IoStreamMessageTimeoutInMilliseconds;

            // assert
            Assert.That(messageTimeoutInMilliseconds, Is.EqualTo(30000));
        }
    }
}
