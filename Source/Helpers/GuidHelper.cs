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

namespace TheCodeKing.Net.Messaging.Helpers
{
    public static class GuidHelper
    {
        #region Public Methods

        public static bool TryParse(string value, out Guid guid)
        {
            try
            {
                var parsedValue = new Guid(value);
                guid = parsedValue;
                return true;
            }
            catch (ArgumentNullException)
            {
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }
            guid = Guid.Empty;
            return false;
        }

        #endregion
    }
}