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
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;

namespace XDMessaging.Fluent
{
    public abstract class XDRegisterations
    {
        #region Constants and Fields

        private readonly IocContainer container;

        #endregion

        #region Constructors and Destructors

        protected XDRegisterations(IocContainer container)
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