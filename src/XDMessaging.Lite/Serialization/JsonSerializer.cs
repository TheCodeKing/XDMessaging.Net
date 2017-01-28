using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Conditions;

namespace XDMessaging.Serialization
{
    public sealed class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string data) where T : class
        {
            data.Requires("data").IsNotNullOrWhiteSpace();

            var serializer = new DataContractJsonSerializer(typeof (T));
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return serializer.ReadObject(stream) as T;
            }
        }

        public string Serialize<T>(T obj) where T : class
        {
            obj.Requires("obj").IsNotNull();

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof (T));
                serializer.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}