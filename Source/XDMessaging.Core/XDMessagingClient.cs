using XDMessaging.Fluent;
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

            Listeners = new Listeners(container);
            Broadcasters = new Broadcasters(container);
        }
    }
}