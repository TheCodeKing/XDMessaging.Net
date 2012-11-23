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

namespace TheCodeKing.Utils.Serialization
{
    public interface ISerializer
    {
        #region Public Methods

        T Deserialize<T>(string data) where T : class;
        string Serialize<T>(T obj)  where T : class;

        #endregion
    }
}