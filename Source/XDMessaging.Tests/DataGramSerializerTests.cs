using NUnit.Framework;
using XDMessaging.Core;
using XDMessaging.Core.Message;
using XDMessaging.Core.Serialization;

namespace XDMessaging.Tests
{
    [TestFixture]
    public class DataGramSerializerTests
    {
        #region Constants and Fields

        private const string channel = "myChannel";
        private const string message = "myMessage";
        private JsonSerializer serializer;

        #endregion

        #region Public Methods

        [Test]
        public void GivenADatagramThenWhenSerializedAssertSuccess()
        {
            // arrange
            var dataGram = new DataGram(channel, message);

            // act
            var result = serializer.Serialize(dataGram);

            // assert
            Assert.That(result, Is.StringContaining(channel));
            Assert.That(result, Is.StringContaining(message));
            Assert.That(result, Is.StringContaining("1.0"));
        }

        [Test]
        public void GivenASerializedDataGramThenWhenDeserializedAssertSuccess()
        {
            // arrange
            const string msg = "{\"ver\":\"2.0\",\"channel\":\"" + channel + "\",\"message\":\"" + message + "\"}";

            // act
            var dataGram = serializer.Deserialize<DataGram>(msg);

            // assert
            Assert.That(dataGram, Is.Not.Null);
            Assert.That(dataGram.Channel, Is.EqualTo(channel));
            Assert.That(dataGram.Message, Is.EqualTo(message));
            Assert.That(dataGram.Version, Is.EqualTo("2.0"));
        }

        [SetUp]
        public void SetUp()
        {
            serializer = new JsonSerializer();
        }

        #endregion
    }
}