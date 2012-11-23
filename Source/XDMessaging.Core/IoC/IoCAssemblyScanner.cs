using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TheCodeKing.Utils.IoC;

namespace XDMessaging.Core.IoC
{
    public sealed class IoCAssemblyScanner
    {
        #region Public Methods

        public void ScanAllAssemblies(IoCContainer container)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ScanAllAssemblies(container, location);
        }

        public void ScanAllAssemblies(IoCContainer container, string location)
        {
            var assemblies = Directory.GetFiles(location, "*.dll").Select(Assembly.LoadFile).ToList();
            foreach (var assembly in assemblies)
            {
                SearchAssemblyForAutoRegistrations(container, assembly);
            }
        }

        #endregion

        #region Methods

        private static void RegisterSpecializedImplementation<T>(IoCContainer container, Type type)
        {
            foreach (var transportMode in type.GetCustomAttributes(typeof (TransportModeHintAttribute), false)
                .Select(attr => ((TransportModeHintAttribute) attr).Mode))
            {
                container.Register(typeof (T), type, Convert.ToString(transportMode));
            }
        }

        private static void SearchAssemblyForAutoRegistrations(IoCContainer container, Assembly assembly)
        {
            var broadcastType = typeof (IXDBroadcast);
            var listenerType = typeof (IXDListener);

            foreach (var type in assembly.GetTypes())
            {
                if (broadcastType.IsAssignableFrom(type))
                {
                    RegisterSpecializedImplementation<IXDBroadcast>(container, type);
                }
                else if (listenerType.IsAssignableFrom(type))
                {
                    RegisterSpecializedImplementation<IXDListener>(container, type);
                }
            }
        }

        #endregion
    }
}