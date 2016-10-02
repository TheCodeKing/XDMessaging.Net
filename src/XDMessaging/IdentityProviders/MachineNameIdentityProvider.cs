using System;

namespace XDMessaging.IdentityProviders
{
    internal sealed class MachineNameIdentityProvider : IIdentityProvider
    {
        public string GetUniqueId()
        {
            return Environment.MachineName;
        }

        public IdentityScope Scope => IdentityScope.Machine;
    }
}