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
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;

namespace XDMessaging.Entities
{
    public abstract class XDRegisterBase
    {
        #region Constants and Fields

        private readonly IocContainer container;

        #endregion

        #region Constructors and Destructors

        protected XDRegisterBase(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            this.container = container;
        }

        #endregion

        #region Properties

        public IocContainer Container
        {
            get { return container; }
        }

        #endregion
    }
}