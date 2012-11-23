using System;
using TheCodeKing.Utils.IoC;
using TheCodeKing.Utils.Serialization;
using XDMessaging.Core.Serialization;
using XDMessaging.Core.Specialized;

namespace XDMessaging.Core.IoC
{
    public sealed class SimpleIoCContainerBootstrapper
    {
        #region Constants and Fields

        private static readonly Lazy<IoCContainer> instance =
            new Lazy<IoCContainer>(() => new SimpleIoCContainer(c => new IoCActivator(c)).Initialize(Configure));

        private static readonly Lazy<IoCAssemblyScanner> scanner = new Lazy<IoCAssemblyScanner>();

        #endregion

        #region Properties

        public static IoCAssemblyScanner Scan
        {
            get { return scanner.Value; }
        }

        #endregion

        #region Public Methods

        public static IoCContainer GetInstance()
        {
            return instance.Value;
        }

        #endregion

        #region Methods

        public static void Configure(IoCContainer container)
        {
            const string binarySerializer = "Binary";
            const string jsonSerializer = "Json";
            container.Register<ISerializer, JsonSerializer>(jsonSerializer);
            container.Register<ISerializer, BinaryBase64Serializer>(binarySerializer);
            container.Register<ISerializer>(
                () => new SpecializedSerializer(container.Resolve<ISerializer>(binarySerializer),
                                                container.Resolve<ISerializer>(jsonSerializer)));
            Scan.ScanAllAssemblies(container, AppDomain.CurrentDomain.BaseDirectory);
        }

        #endregion
    }
}