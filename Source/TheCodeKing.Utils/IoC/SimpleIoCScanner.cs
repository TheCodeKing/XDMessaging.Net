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

namespace XDMessaging.IoC
{
    public class SimpleIocScanner : IocScanner
    {
        protected readonly IocContainer Container;

        private static readonly IList<string> checkedAssemblies = new List<string>();
        private static readonly IDictionary<string, Assembly> dynamicAssemblies = new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly IDictionary<string, Type> foundInterfaces = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        static SimpleIocScanner()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => dynamicAssemblies.ContainsKey(args.Name)
                                                                             ? dynamicAssemblies[args.Name]
                                                                             : null;
        }

        public SimpleIocScanner(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            Container = container;
        }


        public void ScanAllAssemblies()
        {
            ScanLoadedAssemblies();
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ScanAllAssemblies(location);
            location = AppDomain.CurrentDomain.BaseDirectory;
            ScanAllAssemblies(location);
        }

        public void ScanLoadedAssemblies()
        {
            ScanAllAssemblies(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("system.", StringComparison.InvariantCultureIgnoreCase)));
        }

        public void ScanAllAssemblies(string location)
        {
            Validate.That(location).IsNotNullOrEmpty();

            var assemblies = Directory.GetFiles(location, "*.dll")
                .Where(a => (!AppDomain.CurrentDomain.GetAssemblies()
                    .Any(x => AssemblyName.ReferenceMatchesDefinition(AssemblyName.GetAssemblyName(a), x.GetName()))))
                    .Select(Assembly.LoadFrom);
            ScanAllAssemblies(assemblies);
        }

        private void ScanAllAssemblies(IEnumerable<Assembly> assemblies)
        {
            var list = assemblies.ToList();
            var resources = new List<Assembly>();
            foreach (var item in list)
            {
                if ((Path.GetFileNameWithoutExtension(item.Location)??"").StartsWith("System.", StringComparison.InvariantCultureIgnoreCase) || checkedAssemblies.Contains(item.FullName))
                {
                    continue;
                }
                resources.AddRange(SearchResourcesForEmbeddedAssemblies(item));
            }
            list = list.Concat(resources).ToList();
            SearchAssembliesForAllInterfaces(list);
            RegisterConcreteBasedOnInitializeAttribute(list);
            RegisterConcreteBasedOnNamingConvention(list);
        }

        private void RegisterConcreteBasedOnNamingConvention(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var concrete in GetFilteredTypes(assembly))
                {
                    if (foundInterfaces.ContainsKey(concrete.Name))
                    {
                        var interfaceType = foundInterfaces[concrete.Name];
                        if (interfaceType.IsAssignableFrom(concrete))
                        {
                            Container.Register(interfaceType, concrete);
                        }
                        foundInterfaces.Remove(concrete.Name);
                    }
                }
            }
        }

        private void RegisterConcreteBasedOnInitializeAttribute(IEnumerable<Assembly> assemblies)
        {
            foreach(var assembly in assemblies)
            {
                foreach (var concrete in GetFilteredTypes(assembly))
                {
                    var attribute =
                        concrete.GetCustomAttributes(typeof (IocInitializeAttribute), true).FirstOrDefault() as
                        IocInitializeAttribute;
                    if (attribute != null)
                    {
                        InitializeType(concrete, attribute);
                        if (attribute.RegisterType != null)
                        {
                            if (string.IsNullOrEmpty(attribute.Name))
                            {
                                Container.Register(attribute.RegisterType, concrete);
                            }
                            else
                            {
                                Container.Register(attribute.RegisterType, concrete, attribute.Name);
                            }

                        }
                    }
                }
            }
        }

        private static void SearchAssembliesForAllInterfaces(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var item in GetFilteredTypes(assembly).Where(a => a.IsInterface).Where(t => t.Name.StartsWith("I")))
                {
                    foundInterfaces[item.Name.Substring(1)] = item;
                }
            }
        }

        private static IEnumerable<Type> GetFilteredTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => !t.FullName.StartsWith("System.", StringComparison.InvariantCultureIgnoreCase));
        }

        private static IEnumerable<Assembly> SearchResourcesForEmbeddedAssemblies(Assembly assembly)
        {
            var resources = new List<Assembly>();
            foreach (var resource in assembly.GetManifestResourceNames().Where(r => r.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)))
            {
                using (var input = assembly.GetManifestResourceStream(resource))
                {
                    if (input != null)
                    {
                        var dynamicAssembly = Assembly.Load(StreamToBytes(input));
                        if (!dynamicAssemblies.ContainsKey(dynamicAssembly.FullName))
                        {
                            dynamicAssemblies[dynamicAssembly.FullName] = dynamicAssembly;
                        }
                        resources.Add(dynamicAssembly);
                    }
                }
            }
            return resources;
        }

        protected virtual void InitializeType(Type type, IocInitializeAttribute attribute)
        {
            var method = type.GetMethod("Initialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(IocContainer) }, null);
            if (method != null)
            {
                method.Invoke(null, new[] { Container });
            }
        }

        public virtual void ScanAssembly(Assembly assembly)
        {
            Validate.That(assembly).IsNotNull();

            IEnumerable<Assembly> resources;
            if (!checkedAssemblies.Contains(assembly.FullName))
            {
                checkedAssemblies.Add(assembly.FullName);
                resources = SearchResourcesForEmbeddedAssemblies(assembly);
                SearchAssembliesForAllInterfaces(resources.Concat(new[] { assembly }));

                RegisterConcreteBasedOnInitializeAttribute(resources);
                RegisterConcreteBasedOnNamingConvention(resources);
            }

        }
        public void ScanEmbeddedResources(Assembly assembly)
        {
            Validate.That(assembly).IsNotNull();

            foreach(var resources in SearchResourcesForEmbeddedAssemblies(assembly))
            {
                ScanAssembly(resources);
            }
        }

        private static byte[] StreamToBytes(Stream input)
        {
            var capacity = input.CanSeek ? (int)input.Length : 0;
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
    }
}