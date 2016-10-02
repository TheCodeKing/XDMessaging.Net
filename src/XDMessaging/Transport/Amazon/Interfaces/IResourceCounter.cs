namespace XDMessaging.Transport.Amazon.Interfaces
{
    public interface IResourceCounter
    {
        int Decrement(string name);
        void Increment(string name);
    }
}