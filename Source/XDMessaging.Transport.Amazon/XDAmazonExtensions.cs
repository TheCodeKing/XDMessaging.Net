using TheCodeKing.Utils.IoC;
using XDMessaging.Transport.Amazon;

namespace XDMessaging.Core
{
    public static class XDAmazonExtensions
    {
        #region Public Methods

        public static AmazonAccountSettings AwsSettings(this IoCContainer broadcast)
        {
            return AmazonAccountSettings.GetInstance();
        }

        #endregion
    }
}