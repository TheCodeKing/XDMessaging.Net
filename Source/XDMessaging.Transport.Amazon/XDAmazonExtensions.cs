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
using TheCodeKing.Utils.IoC;
using XDMessaging.Transport.Amazon;

namespace XDMessaging.Core
{
    public static class XDAmazonExtensions
    {
        #region Public Methods

        public static AmazonAccountSettings AwsSettings(this IocContainer broadcast)
        {
            return AmazonAccountSettings.GetInstance();
        }

        #endregion
    }
}