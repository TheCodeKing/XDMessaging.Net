using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;

namespace XDMessaging.Core.Serialization
{
    internal sealed class JsonSerializer : ISerializer
    {
        #region Implemented Interfaces

        #region ISerializer

        public T Deserialize<T>(string data) where T : class
        {
            Validate.That(data).IsNotNullOrEmpty();

            var serializer = new DataContractJsonSerializer(typeof (T));
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return serializer.ReadObject(stream) as T;
            }
        }

        public string Serialize<T>(T obj) where T : class
        {
            Validate.That(obj).IsNotNull();

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof (T));
                serializer.WriteObject(stream, obj);
                return Encoding.Default.GetString(stream.ToArray());
            }
        }

        #endregion

        #endregion
    }
}