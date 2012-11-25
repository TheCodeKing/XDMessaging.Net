using System;
using TheCodeKing.Utils.IoC;

namespace XDMessaging.Fluent
{
    public sealed class Broadcasters : XDRegisterations
    {
        #region Constructors and Destructors

        public Broadcasters(IocContainer container)
            : base(container)
        {
        }

        #endregion

        #region Public Methods

        public IXDBroadcaster GetBroadcasterForMode(XDTransportMode transportMode)
        {
            var broadcast = Container.Resolve<IXDBroadcaster>(Convert.ToString(transportMode));
            if (broadcast == null)
            {
                throw new NotSupportedException(
                    string.Format(
                        "No concrete IXDBroadcast for mode {0} could be loaded. Install the {0} assembly in the program directory.",
                        transportMode));
            }
            return broadcast;
        }

        #endregion
    }
}