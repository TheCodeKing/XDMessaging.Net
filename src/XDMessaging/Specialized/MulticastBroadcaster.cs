using System.Collections.Generic;
using System.Linq;
using Conditions;

namespace XDMessaging.Specialized
{
    internal sealed class MulticastBroadcaster : IXDBroadcaster
    {
        private readonly IEnumerable<IXDBroadcaster> broadcasters;

        public MulticastBroadcaster(IEnumerable<IXDBroadcaster> broadcasters)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            broadcasters.Requires("broadcasters").IsNotNull();

            // ReSharper disable once PossibleMultipleEnumeration
            this.broadcasters = broadcasters;
        }

        public MulticastBroadcaster(params IXDBroadcaster[] broadcasters)
            : this((IEnumerable<IXDBroadcaster>) broadcasters)
        {
        }

        public void SendToChannel(string channel, string message)
        {
            foreach (var broadcaster in broadcasters)
            {
                if (broadcaster.IsAlive)
                {
                    broadcaster.SendToChannel(channel, message);
                }
            }
        }

        public void SendToChannel(string channel, object message)
        {
            foreach (var broadcaster in broadcasters)
            {
                if (broadcaster.IsAlive)
                {
                    broadcaster.SendToChannel(channel, message);
                }
            }
        }

        public bool IsAlive
        {
            get { return broadcasters.Any(x => x.IsAlive); }
        }
    }
}