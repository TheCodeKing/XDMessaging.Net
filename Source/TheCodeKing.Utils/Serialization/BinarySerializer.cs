/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TheCodeKing.Utils.Contract;

namespace TheCodeKing.Utils.Serialization
{
    public sealed class BinaryBase64Serializer : ISerializer
    {
        #region Implemented Interfaces

        #region ISerializer<ISerializable>

        public T Deserialize<T>(string data) where T : class
        {
            Validate.That(data, "data").IsNotNullOrEmpty();

            return DeserializeObject(data) as T;
        }

        public string Serialize<T>(T obj) where T : class
        {
            Validate.That(obj, "obj").IsNotNull();

            var type = obj.GetType();
            if (!type.IsSerializable && !(type is ISerializable))
            {
                throw new InvalidOperationException(string.Format("{0} is not a serializable type.", type.FullName));
            }

            var bytes = SerializeBinary(obj);
            return Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
        }

        #endregion

        #endregion

        #region Methods

        private static object DeserializeBinary(byte[] bytes)
        {
            using (var rs = new MemoryStream(bytes, 0, bytes.Length))
            {
                var sf = new BinaryFormatter();
                return sf.Deserialize(rs);
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

        #endregion
    }
}