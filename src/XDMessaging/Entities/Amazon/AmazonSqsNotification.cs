using System.Runtime.Serialization;

namespace XDMessaging.Entities.Amazon
{
    [DataContract]
    internal sealed class AmazonSqsNotification
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Subject { get; set; }
    }
}