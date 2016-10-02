using System.Collections.Generic;
using XDMessaging.Entities;
using XDMessaging.Specialized;

// ReSharper disable once CheckNamespace
namespace XDMessaging
{
    public static partial class RegisterWithClient
    {
        public static IXDBroadcaster GetMulticastBroadcaster(this Broadcasters client,
            params IXDBroadcaster[] broadcasters)
        {
            return new MulticastBroadcaster(broadcasters);
        }

        public static IXDBroadcaster GetMulticastBroadcaster(this Broadcasters client,
            IEnumerable<IXDBroadcaster> broadcasters)
        {
            return new MulticastBroadcaster(broadcasters);
        }
    }
}