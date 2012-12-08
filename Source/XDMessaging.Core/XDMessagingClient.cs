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