using System;
using NUnit.Framework;
using XDMessaging.Messages;
using XDMessaging.Serialization;

namespace XDMessaging.Test
{
    [TestFixture]
    public class DataGramSerializerTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            serializer = new JsonSerializer();
        }

        #endregion

        private const string channel = "myChannel";
        private const string assemblyQualifiedName = "myAssemblyQualifiedName";
        private const string message = "myMessage";
        private JsonSerializer serializer;

        [Test]
        public void GivenADatagramWithSpecialCharsWhenSerializedThenSuccess()
        {
            // arrange
            var specialMsg = "汉字";
            var dataGram = new DataGram(channel, assemblyQualifiedName, specialMsg);

            // act
            var result = serializer.Serialize(dataGram);

            // assert
            Assert.That(result, Is.StringContaining(channel));
            Assert.That(result, Is.StringContaining(specialMsg));
            Assert.That(result, Is.StringContaining(assemblyQualifiedName));
            Assert.That(result, Is.StringContaining("1.1"));
        }

        [Test]
        public void GivenADatagramWhenSerializedThenSuccess()
        {
            // arrange
            var dataGram = new DataGram(channel, assemblyQualifiedName, message);

            // act
            var result = serializer.Serialize(dataGram);

            // assert
            Assert.That(result, Is.StringContaining(channel));
            Assert.That(result, Is.StringContaining(message));
            Assert.That(result, Is.StringContaining(assemblyQualifiedName));
            Assert.That(result, Is.StringContaining("1.1"));
        }

        [Test]
        public void GivenASerializedDataGramVer2WhenDeserializedThenSuccess()
        {
            // arrange
            const string msg =
                "{\"ver\":\"2.0\",\"type\":\"" + assemblyQualifiedName + "\",\"channel\":\"" + channel +
                "\",\"message\":\"" + message + "\"}";

            // act
            var dataGram = serializer.Deserialize<DataGram>(msg);

            // assert
            Assert.That(dataGram, Is.Not.Null);
            Assert.That(dataGram.Channel, Is.EqualTo(channel));
            Assert.That(dataGram.Message, Is.EqualTo(message));
            Assert.That(dataGram.AssemblyQualifiedName, Is.EqualTo(assemblyQualifiedName));
            Assert.That(dataGram.Version, Is.EqualTo("2.0"));
        }

        [Test]
        public void GivenASerializedDataGramVersion1WhenDeserializedThenSuccess()
        {
            // arrange
            const string msg = "{\"ver\":\"1.0\",\"channel\":\"" + channel + "\",\"message\":\"" + message + "\"}";

            // act
            var dataGram = serializer.Deserialize<DataGram>(msg);

            // assert
            Assert.That(dataGram, Is.Not.Null);
            Assert.That(dataGram.Channel, Is.EqualTo(channel));
            Assert.That(dataGram.Message, Is.EqualTo(message));
            Assert.That(dataGram.Version, Is.EqualTo("1.0"));
            string result;
            Assert.Throws<NotSupportedException>(() => result = dataGram.AssemblyQualifiedName);
        }
    }
}