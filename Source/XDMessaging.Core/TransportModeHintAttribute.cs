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

namespace XDMessaging.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TransportModeHintAttribute : Attribute
    {
        #region Constants and Fields

        private readonly XDTransportMode mode;

        #endregion

        #region Constructors and Destructors

        public TransportModeHintAttribute(XDTransportMode mode)
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