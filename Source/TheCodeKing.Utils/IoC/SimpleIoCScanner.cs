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
using System.Text.RegularExpressions;
using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IO;
using TheCodeKing.Utils.IoC;

namespace XDMessaging.IoC
{
    public class SimpleIocScanner : IocScanner
    {
        protected readonly IocContainer Container;

        private static readonly Regex SystemRegex = new Regex(@"^system\.$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
        private const string DefaultSearchPattern = "*.dll";
        private static readonly IList<string> CheckedAssemblies = new List<string>();
        private static readonly IDictionary<string, Assembly> DynamicAssemblies = new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly IDictionary<string, Type> FoundInterfaces = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        static SimpleIocScanner()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => DynamicAssemblies.ContainsKey(args.Name)
                                                                             ? DynamicAssemblies[args.Name]
                                                                             : null;
        }

        public SimpleIocScanner(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            Container = container;
        }

        public void ScanAllAssemblies()
        {
            ScanAllAssemblies(DefaultSearchPattern);
        }

        public void ScanAllAssemblies(string searchPattern)
        {
            Validate.That(searchPattern).IsNotNullOrEmpty();

            var location = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)??"").Trim('\\');
            location = ResolveBinLocation(location);
            ScanAssemblies(location, searchPattern);
            var baselocation = AppDomain.CurrentDomain.BaseDirectory.Trim('\\');
            if (string.Compare(baselocation, location, true) != 0)
            {
                baselocation = ResolveBinLocation(baselocation);
                ScanAssemblies(baselocation, searchPattern);
            }

        }

        public void ScanLoadedAssemblies()
        {
            ScanLoadedAssemblies(DefaultSearchPattern);
        }

        public void ScanLoadedAssemblies(string searchPattern)
        {
            var regex = new Regex("^" + Regex.Escape(searchPattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
            ScanAssemblies(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => regex.IsMatch(Path.GetFileName(a.Location) ?? "")));
        }

        public void ScanAssemblies(string location)
        {
            ScanAssemblies(location, DefaultSearchPattern);
        }

        public void ScanAssemblies(string location, string searchPattern)
        {
            Validate.That(location).IsNotNullOrEmpty();
            Validate.That(searchPattern).IsNotNullOrEmpty();

            var assemblies = Directory.GetFiles(location, searchPattern, SearchOption.AllDirectories)
                .Select(Assembly.LoadFrom)
                .WrapExceptions(e => new IocScannerException("Error loading transport assembly.", e));
            ScanAssemblies(assemblies);
        }

        private void ScanAssemblies(IEnumerable<Assembly> assemblies)
        {

            var list = assemblies.ToList();
            var resources = new List<Assembly>();
            foreach (var item in list)
            {
                if (SystemRegex.IsMatch(Path.GetFileNameWithoutExtension(item.Location) ?? "") || CheckedAssemblies.Contains(item.FullName))
                {
                    continue;
                }
                resources.AddRange(SearchResourcesForEmbeddedAssemblies(item));
            }
            list = list.Concat(resources).DistinctBy(a => a.GetName().Name, StringComparer.InvariantCultureIgnoreCase).ToList();
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
                    if (FoundInterfaces.ContainsKey(concrete.Name))
                    {
                        var interfaceType = FoundInterfaces[concrete.Name];
                        if (interfaceType.IsAssignableFrom(concrete))
                        {
                            Container.Register(interfaceType, concrete);
                        }
                        FoundInterfaces.Remove(concrete.Name);
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
                    FoundInterfaces[item.Name.Substring(1)] = item;
                }
            }
        }

        private static IEnumerable<Type> GetFilteredTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => !SystemRegex.IsMatch(t.FullName??""));
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
                        if (!DynamicAssemblies.ContainsKey(dynamicAssembly.FullName))
                        {
                            DynamicAssemblies[dynamicAssembly.FullName] = dynamicAssembly;
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
            if (!CheckedAssemblies.Contains(assembly.FullName))
            {
                CheckedAssemblies.Add(assembly.FullName);
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

        private static string ResolveBinLocation(string location)
        {
            var testPath = string.Concat(location, @"\bin");
            if (Directory.Exists(testPath))
            {
                return testPath;
            }
            return location;
        }
    }
}