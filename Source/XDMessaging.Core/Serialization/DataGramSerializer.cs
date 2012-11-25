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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;

namespace XDMessaging.Serialization
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