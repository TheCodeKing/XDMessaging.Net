using Conditions;

namespace XDMessaging.Entities.Amazon
{
    internal sealed class Topic
    {
        internal Topic(string topicName, string topicArn)
        {
            topicName.Requires().IsNotNullOrWhiteSpace();
            topicArn.Requires().IsNotNullOrWhiteSpace();

            Name = topicName;
            TopicArn = topicArn;
        }

        public string Name { get; }

        public string TopicArn { get; }
    }
}