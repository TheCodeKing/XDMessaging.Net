using System.Runtime.Serialization;

namespace XDMessaging.Transport.Amazon
{
    [DataContract]
    internal class AmazonSqsNotification
    {
        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}