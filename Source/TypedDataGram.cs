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
using System.Runtime.Serialization;
using TheCodeKing.Net.Messaging.Helpers;

namespace TheCodeKing.Net.Messaging
{
    public class TypedDataGram<T> where T : class 
    {
        #region Constants and Fields

        private readonly DataGram dataGram;
        private readonly SerializerHelper serializeHelper;

        #endregion

        #region Constructors and Destructors

        private TypedDataGram(DataGram dataGram)
        {
            if (dataGram == null)
            {
                throw new ArgumentNullException("dataGram");
            }
            if (!typeof(T).IsSerializable && !(typeof(T) is ISerializable))
            {
                throw new InvalidOperationException(string.Format("{0} is not a serializable type.", typeof(T).FullName));
            }
            serializeHelper = new SerializerHelper();
            this.dataGram = dataGram;
        }

        #endregion

        #region Properties

        public string Channel
        {
            get { return dataGram.Channel; }
        }

        public bool IsValid
        {
            get { return Message != null; }
        }

        public T Message
        {
            get { return serializeHelper.Deserialize<T>(dataGram.Message); }
        }

        #endregion

        #region Operators

        public static implicit operator DataGram(TypedDataGram<T> dataGram)
        {
            if (dataGram == null)
            {
                return null;
            }
            return dataGram.dataGram;
        }

        public static implicit operator TypedDataGram<T>(DataGram dataGram)
        {
            if (dataGram == null)
            {
                return null;
            }
            return new TypedDataGram<T>(dataGram);
        }

        #endregion
    }
}