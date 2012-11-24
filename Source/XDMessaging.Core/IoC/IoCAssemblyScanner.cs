using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;

namespace XDMessaging.Core.IoC
{
    public sealed class IoCAssemblyScanner
    {
        #region Public Methods

        public void ScanAllAssemblies<T>(IoCContainer container)
        {
            Validate.That(container).IsNotNull();

            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ScanAllAssemblies<T>(container, location);
        }

        public void ScanAllAssemblies<T>(IoCContainer container, string location)
        {
            Validate.That(container).IsNotNull();
            Validate.That(location).IsNotNullOrEmpty();

            var assemblies =
                Directory.GetFiles(location, "*.dll").Select(Assembly.LoadFile).Where(
                    a =>
                    !Path.GetFileName(a.Location).StartsWith("system.", StringComparison.InvariantCultureIgnoreCase)).
                    ToList();
            foreach (var assembly in assemblies)
            {
                SearchAssemblyForAutoRegistrations<T>(container, assembly);
            }
        }

        #endregion

        #region Methods

        private static void SearchAssemblyForAutoRegistrations<T>(IoCContainer container, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(type => typeof(T).IsAssignableFrom(type)))
            {
                RegisterSpecializedImplementation<T>(container, type);
            }
        }

        private static void RegisterSpecializedImplementation<T>(IoCContainer container, Type type)
        {
            foreach (var transportMode in type.GetCustomAttributes(typeof(TransportModeHintAttribute), false)
                .Select(attr => ((TransportModeHintAttribute)attr).Mode))
            {
                type.TypeInitializer.Invoke(null, null);
                container.Register(typeof(T), type, Convert.ToString(transportMode));
            }
        }

        #endregion
    }
}