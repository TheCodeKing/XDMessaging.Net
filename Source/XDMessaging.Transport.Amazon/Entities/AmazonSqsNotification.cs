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

namespace XDMessaging.Transport.Amazon.Entities
{
    [DataContract]
    internal sealed class AmazonSqsNotification
    {
        #region Properties

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Subject { get; set; }

        #endregion
    }
}