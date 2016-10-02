namespace XDMessaging.IdentityProviders
{
    internal interface IIdentityProvider
    {
        IdentityScope Scope { get; }

        string GetUniqueId();
    }
}