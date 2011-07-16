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

namespace TheCodeKing.Net.Messaging.Interfaces
{
    internal interface ISerializerHelper
    {
        T Deserialize<T>(string data) where T : class;
        string Serialize(object obj);
    }
}