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
using TheCodeKing.Utils.Contract;

namespace XDMessaging.Transport.Amazon.Entities
{
    internal sealed class Subscriber
    {
        #region Constants and Fields

        private readonly string name;
        private readonly Uri queueUrl;
        private readonly string queueArn;
        private readonly bool longLived;

        #endregion

        #region Constructors and Destructors

        public Subscriber(string name, Uri queueUrl, string queueArn, bool longLived)
        {
            Validate.That(name).IsNotNull();
            Validate.That(queueUrl).IsNotNull();
            Validate.That(queueArn).IsNotNull();

            this.name = name;
            this.queueUrl = queueUrl;
            this.queueArn = queueArn;
            this.longLived = longLived;
        }

        #endregion

        #region Properties

        public bool LongLived
        {
            get { return longLived; }
        }

        public string QueueArn
        {
            get { return queueArn; }
        }

        public Uri QueueUrl
        {
            get { return queueUrl; }
        }

        public string Name
        {
            get { return name; }
        }

        #endregion
    }
}