using System.Runtime.Serialization;
using Conditions;

namespace XDMessaging.Serialization
{
    public class SpecializedSerializer : ISerializer
    {
        private readonly ISerializer dataGramSerializer;
        private readonly ISerializer objectSerializer;

        public SpecializedSerializer(ISerializer objectSerializer, ISerializer dataGramSerializer)
        {
            objectSerializer.Requires("objectSerializer").IsNotNull();
            dataGramSerializer.Requires("dataGramSerializer").IsNotNull();

            this.objectSerializer = objectSerializer;
            this.dataGramSerializer = dataGramSerializer;
        }

        public T Deserialize<T>(string data) where T : class
        {
            data.Requires("data").IsNotNullOrWhiteSpace();

            return SupportsDataContract<T>()
                ? dataGramSerializer.Deserialize<T>(data)
                : objectSerializer.Deserialize<T>(data);
        }

        public string Serialize<T>(T obj) where T : class
        {
            obj.Requires("obj").IsNotNull();

            return SupportsDataContract<T>()
                ? dataGramSerializer.Serialize(obj)
                : objectSerializer.Serialize(obj);
        }

        private static bool SupportsDataContract<T>()
        {
            return typeof (T).GetCustomAttributes(typeof (DataContractAttribute), true).Length > 0;
        }
    }
}