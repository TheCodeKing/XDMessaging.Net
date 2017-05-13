using System.Configuration;

namespace XDMessaging.Config
{
    public static class Settings
    {
        public static uint IoStreamMessageTimeoutInMilliseconds
        {
            get
            {
                var timeoutConfig = ConfigurationManager.AppSettings["IoStreamMessageTimeoutInMilliseconds"];
                if (string.IsNullOrWhiteSpace(timeoutConfig))
                {
                    return 10000;
                }

                uint timeout;
                if (uint.TryParse(timeoutConfig, out timeout))
                {
                    return timeout;
                }

                throw new ConfigurationErrorsException("IoStreamMessageTimeoutInMilliseconds is not a valid int.");
            }
        }
    }
}
