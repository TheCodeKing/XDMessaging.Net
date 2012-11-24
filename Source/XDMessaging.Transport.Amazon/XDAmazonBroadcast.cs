using System;
using System.Configuration;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.Serialization;
using TheCodeKing.Utils.IoC;
using XDMessaging.Core;
using XDMessaging.Core.IoC;
using XDMessaging.Core.Specialized;

namespace XDMessaging.Transport.Amazon
{
    [TransportModeHint(XDTransportMode.RemoteNetwork)]
    public sealed class XDAmazonBroadcast : IXDBroadcast
    {
        #region Constants and Fields

        private readonly IAmazonFacade amazonFacade;
        private readonly ISerializer serializer;

        #endregion

        #region Constructors and Destructors

        static XDAmazonBroadcast()
        {
            var instance = SimpleIoCContainerBootstrapper.GetInstance();
            instance.Register<ISerializer, SpecializedSerializer>();
            instance.Register(() => ConfigurationManager.AppSettings);
            instance.Register(AmazonAccountSettings.GetInstance);
            instance.Register<IAmazonFacade, AmazonFacade>();
        }

        public XDAmazonBroadcast(ISerializer serializer, IAmazonFacade amazonFacade)
        {
            Validate.That(serializer).IsNotNull();
            Validate.That(amazonFacade).IsNotNull();

            this.serializer = serializer;
            this.amazonFacade = amazonFacade;
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcast

        public void SendToChannel(string channel, string message)
        {
            throw new NotImplementedException();
        }

        public void SendToChannel(string channel, object message)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}