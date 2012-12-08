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
using XDMessaging.Entities;
using XDMessaging.IoC;

namespace XDMessaging
{
    public sealed class XDMessagingClient
    {
        public Broadcasters Broadcasters { get; set; }

        public Listeners Listeners { get; set; }

        public XDMessagingClient()
        {
            var container = SimpleIocContainerBootstrapper.GetInstance();

            Listeners = new Listeners(this, container);
            Broadcasters = new Broadcasters(this, container);
        }
    }
}