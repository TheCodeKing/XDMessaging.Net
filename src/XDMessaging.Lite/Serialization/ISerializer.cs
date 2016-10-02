namespace XDMessaging.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(string data) where T : class;
        string Serialize<T>(T obj) where T : class;
    }
}