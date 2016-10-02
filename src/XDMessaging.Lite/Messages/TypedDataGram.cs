using Conditions;
using XDMessaging.Serialization;

namespace XDMessaging.Messages
{
    public class TypedDataGram<T> where T : class
    {
        private readonly DataGram dataGram;
        private readonly ISerializer objectSerializer;

        private TypedDataGram(DataGram dataGram, ISerializer serializer)
        {
            dataGram.Requires("dataGram").IsNotNull();
            serializer.Requires("serializer").IsNotNull();

            objectSerializer = serializer;
            this.dataGram = dataGram;
        }

        public string AssemblyQualifiedName => dataGram.AssemblyQualifiedName;

        public string Channel => dataGram.Channel;

        public bool IsValid => Message != null;

        public T Message => objectSerializer.Deserialize<T>(dataGram.Message);

        public static implicit operator DataGram(TypedDataGram<T> dataGram)
        {
            return dataGram?.dataGram;
        }

        public static implicit operator TypedDataGram<T>(DataGram dataGram)
        {
            return dataGram == null ? null : new TypedDataGram<T>(dataGram, XDMessagingClient.Serializer);
        }
    }
}