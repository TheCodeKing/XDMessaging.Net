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
    public class SimpleIoCScanner : IoCScanner
    {
        protected readonly IocContainer Container;

        private static readonly IList<string> checkedAssemblyResources = new List<string>();
        private static readonly IList<string> checkedAssemblyTypes = new List<string>();
        private static readonly IDictionary<string, Assembly> dynamicAssemblies = new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);

        static SimpleIoCScanner()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => dynamicAssemblies.ContainsKey(args.Name)
                                                                             ? dynamicAssemblies[args.Name]
                                                                             : null;
        }

        public SimpleIoCScanner(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            Container = container;
        }

        public void ScanEmbeddedResources(Assembly assembly)
        {
            Validate.That(assembly).IsNotNull();

            if (IsAssemblyResourcesChecked(assembly))
            {
                return;
            }

            foreach (var resource in assembly.GetManifestResourceNames().Where(r => r.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)))
            {
                using (var input = assembly.GetManifestResourceStream(resource))
                {
                    if (input != null)
                    {
                        var dynamicAssembly = Assembly.Load(StreamToBytes(input));
                        dynamicAssemblies[dynamicAssembly.FullName] = dynamicAssembly;
                        ScanAssembly(dynamicAssembly);
                    }
                }
            }
        }

        public virtual void ScanAssembly(Assembly assembly)
        {
            ScanAssemblyUsingConventions(assembly);
            InitializeAssembliesWithInitializeAttribute(assembly);
        }

        private void InitializeAssembliesWithInitializeAttribute(Assembly assembly)
        {
            foreach (var item in assembly.GetTypes())
            {
                if (item != null)
                {
                    var attribute = item.GetCustomAttributes(typeof(IocInitializeAttribute), true).FirstOrDefault() as IocInitializeAttribute;
                    if (attribute != null)
                    {
                        InitializeType(item, attribute);
                        if (attribute.RegisterType != null && !string.IsNullOrEmpty(attribute.Name))
                        {
                            Container.Register(attribute.RegisterType, item, attribute.Name);
                        }
                    }
                }
            }
        }

        protected virtual void ScanAssemblyUsingConventions(Assembly assembly)
        {
            foreach (var item in assembly.GetTypes())
            {
                if (item != null)
                {
                    if (item.IsInterface)
                    {
                        var concrete = assembly.GetTypes().Where(t => t.Name.StartsWith("I")).Where(t => t.Name == item.Name.Substring(1)).FirstOrDefault();
                        if (concrete != null)
                        {
                            Container.Register(item, concrete);
                        }
                    }
                }
            }
        }

        public void ScanAllAssemblies()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ScanAllAssemblies(location);
        }

        public void ScanAllAssemblies(string location)
        {
            Validate.That(location).IsNotNullOrEmpty();
  
            Directory.GetFiles(location, "*.dll")
                .Select(Assembly.LoadFile)
                .Where(a=>!IsAssemblyTypesChecked(a))
                .ToList().ForEach(ScanAssembly);
        }

        protected virtual void InitializeType(Type type, IocInitializeAttribute attribute)
        {
            var method = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static, null, new []{typeof(IocContainer)}, null );
            if (method != null)
            {
                method.Invoke(null, new[] { Container });
            }
        }

        private static bool IsAssemblyResourcesChecked(Assembly assembly)
        {
            if (!checkedAssemblyResources.Contains(assembly.FullName))
            {
                checkedAssemblyResources.Add(assembly.FullName);
                return false;
            }
            return true;
        }

        private static bool IsAssemblyTypesChecked(Assembly assembly)
        {
            if (!checkedAssemblyTypes.Contains(assembly.FullName))
            {
                checkedAssemblyTypes.Add(assembly.FullName);
                return false;
            }
            return true;
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