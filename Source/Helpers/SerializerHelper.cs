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
using TheCodeKing.Net.Messaging.Interfaces;

namespace TheCodeKing.Net.Messaging.Helpers
{
    internal sealed class SerializerHelper : ISerializerHelper
    {
        #region Implemented Interfaces

        #region ISerializerHelper

        public T Deserialize<T>(string data) where T : class
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException("data");
            }
            if (!typeof (T).IsSerializable && !(typeof (T) is ISerializable))
            {
                throw new InvalidOperationException(string.Format("{0} is not a serializable type.", typeof (T).FullName));
            }
            object item = DeserializeObject(data);
            if (item == null)
            {
                return null;
            }
            return item as T;
        }

        public string Serialize(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            Type type = obj.GetType();
            if (!type.IsSerializable && !(type is ISerializable))
            {
                throw new InvalidOperationException(string.Format("{0} is not a serializable type.", type.FullName));
            }

            try
            {
                byte[] bytes = SerializeBinary(obj);
                return Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
            }
            catch (SerializationException e)
            {
                throw new SerializationException("Unable to serialize message, ensure object is serializable.", e);
            }
        }

        #endregion

        #endregion

        #region Methods

        private static object DeserializeBinary(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            using (var rs = new MemoryStream(bytes, 0, bytes.Length))
            {
                var sf = new BinaryFormatter();
                return sf.Deserialize(rs);
            }
        }

        private static object DeserializeObject(string data)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(data);
                return DeserializeBinary(bytes);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (SerializationException)
            {
                return null;
            }
        }

        private static byte[] SerializeBinary(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            byte[] bytes;
            using (var ws = new MemoryStream())
            {
                var sf = new BinaryFormatter();
                sf.Serialize(ws, obj);
                bytes = ws.GetBuffer();
            }
            return bytes;
        }

        #endregion
    }
}