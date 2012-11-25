using System;
using TheCodeKing.Utils.IoC;

namespace XDMessaging.Fluent
{
    public sealed class Listeners : Implementations
    {
        /// <summary>
        ///   The delegate used for handling cross AppDomain communications.
        /// </summary>
        /// <param name = "sender">The event sender.</param>
        /// <param name = "e">The event args containing the DataGram data.</param>
        public delegate void XDMessageHandler(object sender, XDMessageEventArgs e);

        public IXDListener GetListenerForMode(XDTransportMode transportMode)
        {
            var listener = Container.Resolve<IXDListener>(Convert.ToString(transportMode));
            if (listener == null)
            {
                throw new NotSupportedException(
                    string.Format(
                        "No concrete IXDListener for mode {0} could be loaded. Install the {0} assembly in the program directory.",
                        transportMode));
            }
            return listener;
        }

        #region Constructors and Destructors

        public Listeners(IocContainer container)
            : base(container)
        {
        }

        #endregion
    }
}