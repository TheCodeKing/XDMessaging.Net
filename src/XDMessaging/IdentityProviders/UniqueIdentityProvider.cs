using System;

namespace XDMessaging.IdentityProviders
{
    internal sealed class UniqueIdentityProvider : IIdentityProvider
    {
        public string GetUniqueId()
        {
            return Guid.NewGuid().ToString("N");
        }

        public IdentityScope Scope => IdentityScope.Instance;
    }
}