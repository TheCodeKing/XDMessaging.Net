using System.Runtime.Serialization;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Core.Message;

namespace XDMessaging.Core.Specialized
{
    public class SpecializedSerializer : ISerializer
    {
        #region Constants and Fields

        private readonly ISerializer dataGramSerializer;
        private readonly ISerializer objectSerializer;

        #endregion

        #region Constructors and Destructors

        public SpecializedSerializer(ISerializer objectSerializer, ISerializer dataGramSerializer)
        {
            Validate.That(objectSerializer).IsNotNull();
            Validate.That(dataGramSerializer).IsNotNull();

            this.objectSerializer = objectSerializer;
            this.dataGramSerializer = dataGramSerializer;
        }

        #endregion

        #region Implemented Interfaces

        #region ISerializer<object>

        public T Deserialize<T>(string data) where T : class
        {
            Validate.That(data).IsNotNullOrEmpty();

            if (typeof(T).GetCustomAttributes(typeof (DataContractAttribute), true).Length>0)
            {
                return dataGramSerializer.Deserialize<T>(data); 
            }
            return objectSerializer.Deserialize<T>(data);
        }

        public string Serialize<T>(T obj) where T : class
        {
            Validate.That(obj).IsNotNull();

            if (typeof(T).GetCustomAttributes(typeof(DataContractAttribute), true).Length > 0)
            {
                return dataGramSerializer.Serialize<T>(obj);
            }
            return objectSerializer.Serialize<T>(obj);
        }

        #endregion

        #endregion
    }
}