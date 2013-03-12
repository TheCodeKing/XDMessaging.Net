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
using TheCodeKing.Utils.IoC;

namespace XDMessaging
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XDListenerHintAttribute : IocInitializeAttribute
    {
        #region Constants and Fields

        private readonly XDTransportMode mode;

        #endregion

        #region Constructors and Destructors

        public XDListenerHintAttribute(XDTransportMode mode)
            : base(Convert.ToString(mode), typeof(IXDListener))
        {
            this.mode = mode;
        }

        #endregion

        #region Properties

        public XDTransportMode Mode
        {
            get { return mode; }
        }

        #endregion
    }
}