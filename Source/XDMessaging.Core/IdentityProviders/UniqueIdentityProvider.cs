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
using System;

namespace XDMessaging.IdentityProviders
{
    public sealed class UniqueIdentityProvider : IIdentityProvider
    {
        #region Public Methods

        public IdentityScope Scope
        {
            get { return IdentityScope.Instance; }
        }

        public string GetUniqueId()
        {
            return Guid.NewGuid().ToString("N");
        }

        #endregion
    }
}