using System;
using Conditions;

namespace XDMessaging.Entities.Amazon
{
    internal sealed class Subscriber
    {
        public Subscriber(string name, Uri queueUrl, string queueArn, bool longLived)
        {
            name.Requires("name").IsNotNull();
            queueUrl.Requires("queueUrl").IsNotNull();
            queueArn.Requires("queueArn").IsNotNull();

            Name = name;
            QueueUrl = queueUrl;
            QueueArn = queueArn;
            LongLived = longLived;
        }

        public bool LongLived { get; }

        public string Name { get; }

        public string QueueArn { get; }

        public Uri QueueUrl { get; }
    }
}