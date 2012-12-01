using TheCodeKing.Utils.Contract;

namespace XDMessaging.Transport.Amazon.Entities
{
    internal sealed class Topic
    {
        private readonly string topicName;
        private readonly string topicArn;

        internal Topic(string topicName, string topicArn)
        {
            Validate.That(topicName).IsNotNullOrEmpty();
            Validate.That(topicArn).IsNotNullOrEmpty();

            this.topicName = topicName;
            this.topicArn = topicArn;
        }

        public string TopicArn
        {
            get { return topicArn; }
        }

        public string Name
        {
            get { return topicName; }
        }
    }
}
