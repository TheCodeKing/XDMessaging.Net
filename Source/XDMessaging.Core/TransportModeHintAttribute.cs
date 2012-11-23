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