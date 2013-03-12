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
using XDMessaging.Transport.Amazon;

namespace XDMessaging.Tests
{
    [TestFixture]
    public class ResourceCounterTests
    {
        private readonly string ID = Guid.NewGuid().ToString();

        [Test]
        public void GivenEmptyResourceCounterThenShouldReturnZeroCountOnDecrement()
        {
            var resourceCounter = new ResourceCounter();

            Assert.AreEqual(0, resourceCounter.Decrement(ID));
            Assert.AreEqual(0, resourceCounter.Decrement(ID));
        }

        [Test]
        public void GivenMultipleIncrementThenEachDecrementShouldDecreaseTheCountToZero()
        {
            var resourceCounter = new ResourceCounter();

            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);

            Assert.AreEqual(9, resourceCounter.Decrement(ID));
            Assert.AreEqual(8, resourceCounter.Decrement(ID));
            Assert.AreEqual(7, resourceCounter.Decrement(ID));
            Assert.AreEqual(6, resourceCounter.Decrement(ID));
            Assert.AreEqual(5, resourceCounter.Decrement(ID));
            Assert.AreEqual(4, resourceCounter.Decrement(ID));
            Assert.AreEqual(3, resourceCounter.Decrement(ID));
            Assert.AreEqual(2, resourceCounter.Decrement(ID));
            Assert.AreEqual(1, resourceCounter.Decrement(ID));
            Assert.AreEqual(0, resourceCounter.Decrement(ID));
            Assert.AreEqual(0, resourceCounter.Decrement(ID));
        }

        [Test]
        public void GivenOneIncrementAndOneDecrementThenShouldReturnZeroCountOnDecrement()
        {
            var resourceCounter = new ResourceCounter();

            resourceCounter.Increment(ID);

            Assert.AreEqual(0, resourceCounter.Decrement(ID));
        }

        [Test]
        public void GivenTwoIncrementAndOneDecrementThenShouldReturnOne()
        {
            var resourceCounter = new ResourceCounter();

            resourceCounter.Increment(ID);
            resourceCounter.Increment(ID);

            Assert.AreEqual(1, resourceCounter.Decrement(ID));
            Assert.AreEqual(0, resourceCounter.Decrement(ID));
        }
    }
}