/*=============================================================================
*
*	(C) Copyright 2013, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using System.Runtime.Serialization;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;

namespace XDMessaging.Specialized
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

        #region ISerializer

        public T Deserialize<T>(string data) where T : class
        {
            Validate.That(data).IsNotNullOrEmpty();

            if (typeof (T).GetCustomAttributes(typeof (DataContractAttribute), true).Length > 0)
            {
                return dataGramSerializer.Deserialize<T>(data);
            }
            return objectSerializer.Deserialize<T>(data);
        }

        public string Serialize<T>(T obj) where T : class
        {
            Validate.That(obj).IsNotNull();

            if (typeof (T).GetCustomAttributes(typeof (DataContractAttribute), true).Length > 0)
            {
                return dataGramSerializer.Serialize(obj);
            }
            return objectSerializer.Serialize(obj);
        }

        #endregion

        #endregion
    }
}