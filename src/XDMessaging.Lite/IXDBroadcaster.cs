namespace XDMessaging
{
    // ReSharper disable once InconsistentNaming
    public interface IXDBroadcaster
    {
        bool IsAlive { get; }

        void SendToChannel(string channel, string message);

        void SendToChannel(string channel, object message);
    }
}