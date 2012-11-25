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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;

namespace XDMessaging.Core.IoC
{
    public sealed class IoCAssemblyScanner : IoCScanner
    {
        private readonly IocContainer container;

        #region Constants and Fields

        private static readonly Type broadcastType = typeof (IXDBroadcast);
        private static readonly Type listenerType = typeof (IXDListener);
        private static readonly IDictionary<string, Assembly> dynamicAssemblies = new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);

        #endregion

        public IoCAssemblyScanner(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            this.container = container;
        }

        static IoCAssemblyScanner()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => dynamicAssemblies.ContainsKey(args.Name) ? dynamicAssemblies[args.Name] : null;    
        }

        #region Implemented Interfaces

        #region IoCScanner

        /// <summary>
        ///   Scan assemblies at the same location as the executing assembly for concrete XD implementations.
        /// </summary>
        public void ScanAllAssemblies()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ScanAllAssemblies(location);
        }

        /// <summary>
        ///   Scan assemblies at given location for concrete XD implementations.
        /// </summary>
        /// <param name = "location">The search location.</param>
        public void ScanAllAssemblies(string location)
        {
            Validate.That(location).IsNotNullOrEmpty();

            var assemblies =
                Directory.GetFiles(location, "*.dll").Select(Assembly.LoadFile).Where(
                    a =>
                    !Path.GetFileName(a.Location).StartsWith("system.", StringComparison.InvariantCultureIgnoreCase))
                    .
                    ToList();
            foreach (var assembly in assemblies)
            {
                SearchAssemblyForAutoRegistrations(container, assembly);
            }
        }

        public void ScanEmbeddedAssemblies(Assembly assembly)
        {
            Validate.That(assembly).IsNotNull();

            foreach (var resource in assembly.GetManifestResourceNames())
            {
                if (resource.EndsWith(".dll"))
                {
                    using (var input = assembly.GetManifestResourceStream(resource))
                    {
                        if (input != null)
                        {
                            var dynamicAssembly = Assembly.Load(StreamToBytes(input));
                            if (dynamicAssembly != null)
                            {
                                dynamicAssemblies[dynamicAssembly.FullName] = dynamicAssembly;
                            }
                        }
                    }
                }
            }
        }

        #endregion

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

        private static byte[] StreamToBytes(Stream input)
        {
            var capacity = input.CanSeek ? (int) input.Length : 0;
            using (var output = new MemoryStream(capacity))
            {
                int readLength;
                var buffer = new byte[4096];

                do
                {
                    readLength = input.Read(buffer, 0, buffer.Length);
                    output.Write(buffer, 0, readLength);
                } while (readLength != 0);

                return output.ToArray();
            }
        }

        #endregion
    }
}