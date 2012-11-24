/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
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
        #region Constants and Fields

        private static readonly Type broadcastType = typeof (IXDBroadcast);
        private static readonly Type listenerType = typeof (IXDListener);

        #endregion

        #region Public Methods

        /// <summary>
        /// Scan assemblies at the same location as the executing assembly for concrete XD implementations.
        /// </summary>
        /// <param name="container">The IocContainer to register the mappings.</param>
        public void ScanAllAssemblies(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ScanAllAssemblies(container, location);
        }

        /// <summary>
        /// Scan assemblies at given location for concrete XD implementations.
        /// </summary>
        /// <param name="container">The IocContainer to register the mappings.</param>
        /// <param name="location">The search location.</param>
        public void ScanAllAssemblies(IocContainer container, string location)
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
                SearchAssemblyForAutoRegistrations(container, assembly);
            }
        }

        #endregion

        #region Methods

        private static void InitializeXdType(Type type, IocContainer container)
        {
            var method = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(null, new[] {container});
            }
        }

        private static void RegisterSpecializedImplementation(IocContainer container, Type type)
        {
            foreach (var transportMode in type.GetCustomAttributes(typeof (TransportModeHintAttribute), false)
                .Select(attr => ((TransportModeHintAttribute) attr).Mode))
            {
                if (broadcastType.IsAssignableFrom(type))
                {
                    if (container.IsRegistered(broadcastType))
                    {
                        throw new InvalidOperationException(
                            "A concrete IXDBroadcast is already registered with the IocContainer. Remove conflicting assemblies and ensure only one implementation of each mode is installed.");
                    }
                    // ensure static constructor is executed to enable external impl to register dependencies
                    InitializeXdType(type, container);
                    container.Register(broadcastType, type, Convert.ToString(transportMode));
                }
                if (listenerType.IsAssignableFrom(type))
                {
                    if (container.IsRegistered(listenerType))
                    {
                        throw new InvalidOperationException(
                            "A concrete IXDListener is already registered with the IocContainer. Remove conflicting assemblies and ensure only one implementation of each mode is installed.");
                    }
                    // ensure static constructor is executed to enable external impl to register dependencies
                    InitializeXdType(type, container);
                    container.Register(listenerType, type, Convert.ToString(transportMode));
                }
            }
        }

        private static void SearchAssemblyForAutoRegistrations(IocContainer container, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                RegisterSpecializedImplementation(container, type);
            }
        }

        #endregion
    }
}