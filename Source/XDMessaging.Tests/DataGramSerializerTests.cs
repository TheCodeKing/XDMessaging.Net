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
using NUnit.Framework;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Messages;

namespace XDMessaging.Tests
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
        public void GivenADatagramThenWhenSerializedAssertSuccess()
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
        public void GivenASerializedDataGramThenWhenDeserializedAssertSuccess()
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
        public void GivenASerializedDataGramVersion1ThenWhenDeserializedAssertSuccess()
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