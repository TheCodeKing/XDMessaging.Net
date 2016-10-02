using XDMessaging.Entities;
using XDMessaging.Serialization;

namespace XDMessaging
{
    // ReSharper disable once InconsistentNaming
    public sealed class XDMessagingClient
    {
        internal static readonly ISerializer Serializer;

        static XDMessagingClient()
        {
            Serializer = new SpecializedSerializer(new BinaryBase64Serializer(), new JsonSerializer());
        }

        public XDMessagingClient()
        {
            Listeners = new Listeners(this, Serializer);
            Broadcasters = new Broadcasters(this, Serializer);
        }

        public Broadcasters Broadcasters { get; }

        public Listeners Listeners { get; }
    }
}