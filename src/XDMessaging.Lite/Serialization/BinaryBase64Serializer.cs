using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Conditions;

namespace XDMessaging.Serialization
{
    public sealed class BinaryBase64Serializer : ISerializer
    {
        public T Deserialize<T>(string data) where T : class
        {
            data.Requires("data").IsNotNullOrWhiteSpace();

            return DeserializeObject(data) as T;
        }

        public string Serialize<T>(T obj) where T : class
        {
            obj.Requires("obj").IsNotNull();

            var type = obj.GetType();
            if (!type.IsSerializable && !(type is ISerializable))
            {
                throw new InvalidOperationException($"{type.FullName} is not a serializable type.");
            }

            var bytes = SerializeBinary(obj);
            return Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
        }

        private static object DeserializeBinary(byte[] bytes)
        {
            using (var rs = new MemoryStream(bytes, 0, bytes.Length))
            {
                var sf = new BinaryFormatter();
                try
                {
                    return sf.Deserialize(rs);
                }
                catch (SerializationException)
                {
                    return null;
                }
            }
        }

        private static object DeserializeObject(string data)
        {
            var bytes = Convert.FromBase64String(data);
            return DeserializeBinary(bytes);
        }

        private static byte[] SerializeBinary(object obj)
        {
            using (var ws = new MemoryStream())
            {
                var sf = new BinaryFormatter();
                sf.Serialize(ws, obj);
                return ws.GetBuffer();
            }
        }
    }
}